#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Reflecta;

#endregion

public sealed class Server : IDisposable
{
    #region CustomPerPixelAlphaForm

    public CustomPerPixelAlphaForm Form { get; private set; }

    private void FormSetProperties()
    {
        Form.Enabled = true;
        Form.FormBorderStyle = FormBorderStyle.None;
        Form.ShowInTaskbar = false;
        Form.TopMost = true;
        Form.StartPosition = FormStartPosition.Manual;
    }

    private void FormDock()
    {
        Form.Location = new Point(Screen.PrimaryScreen.Bounds.Width - Constants.SCREEN_WIDTH,
            Screen.PrimaryScreen.Bounds.Height - Constants.SCREEN_HEIGHT);
    }

    public Server()
    {
        Form = new CustomPerPixelAlphaForm();
        FormSetProperties();
        FormDock();
        Form.Show();

        var clientBuildDirectory = Environment.CurrentDirectory + "\\..\\..\\..\\..\\..\\Reflecta.Client\\bin";
        var clientStartInfo = new ProcessStartInfo
        {
            FileName = clientBuildDirectory + "\\Client.exe",
            WorkingDirectory = clientBuildDirectory,
            WindowStyle = ProcessWindowStyle.Minimized
        };
        Client = Process.Start(clientStartInfo);

        OpenPipes();

        SpeechSynthesizer = new SpeechSynthesizer();
        SpeechSynthesizer.SelectVoiceByHints(VoiceGender.Female);
        SpeechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
        SpeechSynthesizer.VisemeReached += SpeechSynthesizer_VisemeReached;
        SpeechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;

        SpeechRecognitionEngine = new SpeechRecognitionEngine();
        SpeechRecognitionEngine.UnloadAllGrammars();
        SpeechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder(KnownCommands)));
        SpeechRecognitionEngine.SpeechRecognized += SpeechRecognitionEngine_SpeechRecognized;
        SpeechRecognitionEngine.SetInputToDefaultAudioDevice();
        SpeechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);

        KinectSensor = KinectSensor.GetDefault();
        KinectSensor.Open();

        BodyFrameSource = KinectSensor.BodyFrameSource;
        BodyFrameReader = BodyFrameSource.OpenReader();
        BodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived;
        Bodies = null;
        BodyDESP = new DESPQuaternion[(int) MoCapKinectBone.Count];
        for (var i = 0; i < (int) MoCapKinectBone.Count; i++)
            BodyDESP[i] = new DESPQuaternion();

        HighDefinitionFaceFrameSource = new HighDefinitionFaceFrameSource(KinectSensor);
        HighDefinitionFaceFrameSource.TrackingQuality = FaceAlignmentQuality.High;
        HighDefinitionFaceFrameReader = HighDefinitionFaceFrameSource.OpenReader();
        HighDefinitionFaceFrameReader.FrameArrived += HighDefinitionFaceFrameReader_FrameArrived;
        FaceAlignment = new FaceAlignment();

        FaceDESP = new DESPQuaternion();
        FaceExpressionDESP = new DESPFloat[(int) MoCapKinectFacialExpression.Count];
        for (var i = 0; i < (int) MoCapKinectFacialExpression.Count; i++)
            FaceExpressionDESP[i] = new DESPFloat();
    }

    private static bool RunAsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    #region IDisposable

    private bool Disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (Disposed)
            return;

        if (disposing)
        {
            //Free any other managed objects here...

            ClosePipes();

            Client.CloseMainWindow();
            Client.Close();
            Client.Dispose();
            Client = null;

            SpeechSynthesizer.SpeakAsyncCancelAll();
            SpeechSynthesizer.Dispose();
            SpeechSynthesizer = null;

            SpeechRecognitionEngine.RecognizeAsyncStop();
            SpeechRecognitionEngine.Dispose();
            SpeechRecognitionEngine = null;

            HighDefinitionFaceFrameReader.Dispose();
            HighDefinitionFaceFrameReader = null;
            HighDefinitionFaceFrameSource = null;

            BodyFrameReader.Dispose();
            BodyFrameReader = null;
            BodyFrameSource = null;

            KinectSensor.Close();
            KinectSensor = null;

            Form.Close();
            Form.Dispose();
            Form = null;
        }

        //Free any unmanaged objects here...

        Disposed = true;
    }

    ~Server()
    {
        Dispose(false);
    }

    #endregion

    #endregion

    #region NamedPipeServerStream

    public Process Client { get; private set; }

    public NamedPipeServerStream RenderingStream { get; private set; }
    public NamedPipeServerStream CommandStream { get; private set; }

    private readonly object Lock = new object();
    private readonly byte[] Pixels = new byte[Constants.SCREEN_BYTE_COUNT];

    private readonly Bitmap Frame = new Bitmap(Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT,
        PixelFormat.Format32bppArgb);

    private void OpenPipes()
    {
        RenderingStream = new NamedPipeServerStream("Rendering Pipeline", PipeDirection.In, 1, PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous, Constants.RENDERING_PIPELINE_BUFFER_SIZE, 0);
        RenderingStream.BeginWaitForConnection(RenderingPipelineConnectedAsyncCallback, null);

        CommandStream = new NamedPipeServerStream("Command Pipeline", PipeDirection.Out, 1, PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous, 0, Constants.COMMAND_PIPELINE_BUFFER_SIZE);
        CommandStream.BeginWaitForConnection(CommandPipelineConnectedAsyncCallback, null);
    }

    private volatile bool RenderingPipelineConnectedAsyncCallbackFinished;

    private void RenderingPipelineConnectedAsyncCallback(IAsyncResult result)
    {
        RenderingStream.EndWaitForConnection(result);
        RenderingPipelineConnectedAsyncCallbackFinished = true;

        if (RenderingPipelineConnectedAsyncCallbackFinished && CommandPipelineConnectedAsyncCallbackFinished)
            BeginReadPixels();
    }

    private volatile bool CommandPipelineConnectedAsyncCallbackFinished;

    private void CommandPipelineConnectedAsyncCallback(IAsyncResult result)
    {
        CommandStream.EndWaitForConnection(result);
        CommandPipelineConnectedAsyncCallbackFinished = true;

        if (RenderingPipelineConnectedAsyncCallbackFinished && CommandPipelineConnectedAsyncCallbackFinished)
            BeginReadPixels();
    }

    private void ClosePipes()
    {
        RenderingStream.Close();
        RenderingStream.Dispose();
        RenderingStream = null;

        CommandStream.Close();
        CommandStream.Dispose();
        CommandStream = null;
    }

    private void BeginWriteCommand(CommandMessage command)
    {
        if (CommandStream != null && CommandStream.IsConnected)
        {
            var commandBytes = CommandMessage.ToBytes(command);

            CommandStream.WaitForPipeDrain();

            CommandStream.BeginWrite(commandBytes, 0, commandBytes.Length, WriteCommandAsyncCallback, null);
        }
    }

    private void WriteCommandAsyncCallback(IAsyncResult result)
    {
        if (CommandStream != null && CommandStream.IsConnected)
        {
            CommandStream.EndWrite(result);

            CommandStream.Flush();
        }
    }

    private void ProcessFrame()
    {
        //unchecked
        //{
        //    //Flip image vertically...
        //    int bufferLength = Constants.SCREEN_WIDTH * 4;

        //    Parallel.For(0, Constants.SCREEN_HEIGHT / 2, y =>
        //    {
        //        byte[] auxBuffer = new byte[bufferLength];

        //        Buffer.BlockCopy(Pixels, bufferLength * y, auxBuffer, 0, bufferLength);
        //        Buffer.BlockCopy(Pixels, bufferLength * (Constants.SCREEN_HEIGHT - 1 - y), Pixels, bufferLength * y, bufferLength);
        //        Buffer.BlockCopy(auxBuffer, 0, Pixels, bufferLength * (Constants.SCREEN_HEIGHT - 1 - y), bufferLength);
        //    });

        //    //Swap image R and B channels...
        //    Parallel.For(0, Constants.SCREEN_BYTE_COUNT / 4, index =>
        //    {
        //        int auxIndex = index * 4;

        //        byte auxByte = 0;

        //        auxByte = Pixels[auxIndex];
        //        Pixels[auxIndex] = Pixels[auxIndex + 2];
        //        Pixels[auxIndex + 2] = auxByte;

        //        auxByte = Pixels[auxIndex + 3];
        //        Pixels[auxIndex + 3] = auxByte > (byte)0 ? (byte)255 : (byte)0;
        //    });
        //}
    }

    private void BeginReadPixels()
    {
        if (RenderingStream != null && RenderingStream.IsConnected)
        {
            RenderingStream.BeginRead(Pixels, 0, Constants.SCREEN_BYTE_COUNT, ReadPixelsAsyncCallback, null);
        }
    }

    private void ReadPixelsAsyncCallback(IAsyncResult result)
    {
        if (RenderingStream != null && RenderingStream.IsConnected)
        {
            var amountRead = RenderingStream.EndRead(result);

            if (amountRead != Constants.SCREEN_BYTE_COUNT)
                throw new Exception("Amount read mismatch!");

            lock (Lock)
            {
                ProcessFrame();

                var data = Frame.LockBits(new Rectangle(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT),
                    ImageLockMode.WriteOnly, Frame.PixelFormat);
                Marshal.Copy(Pixels, 0, data.Scan0, Constants.SCREEN_BYTE_COUNT);
                Frame.UnlockBits(data);
            }

            if (Form.IsHandleCreated)
                Form.BeginInvoke(new Action(() =>
                {
                    lock (Lock)
                    {
                        Form.SetBitmap(Frame);
                    }
                }));
        }

        BeginReadPixels();
    }

    #endregion

    #region SpeechSynthesizer

    public SpeechSynthesizer SpeechSynthesizer { get; private set; }

    private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
    {
        DrawViseme(0);
    }

    private void SpeechSynthesizer_VisemeReached(object sender, VisemeReachedEventArgs e)
    {
        DrawViseme(e.Viseme);
    }

    private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
    {
        DrawViseme(0);
    }

    private void DrawViseme(int id)
    {
        var command = new CommandMessage
        {
            CommandType = CommandType.Viseme,
            Viseme = (Viseme) id
        };

        BeginWriteCommand(command);
    }

    #endregion

    #region SpeechRecognitionEngine

    public SpeechRecognitionEngine SpeechRecognitionEngine { get; private set; }

    private static readonly Choices KnownCommands = new Choices("forward", "backward", "up", "down", "right", "left",
        "body", "face");

    private const float SPEECH_RECOGNITION_CONFIDENCE_THRESHOLD = 0.925f;

    private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
        if (e.Result.Confidence < SPEECH_RECOGNITION_CONFIDENCE_THRESHOLD)
            return;

        var command = new CommandMessage
        {
            CommandType = CommandType.VoiceCommand,
            VoiceCommand = e.Result.Text
        };

        BeginWriteCommand(command);
    }

    #endregion

    #region Kinect

    public KinectSensor KinectSensor { get; private set; }

    private BodyFrameSource BodyFrameSource;
    private BodyFrameReader BodyFrameReader;
    private IList<Body> Bodies;
    private readonly DESPQuaternion[] BodyDESP;

    private HighDefinitionFaceFrameSource HighDefinitionFaceFrameSource;
    private HighDefinitionFaceFrameReader HighDefinitionFaceFrameReader;
    private readonly FaceAlignment FaceAlignment;
    private readonly DESPQuaternion FaceDESP;
    private readonly DESPFloat[] FaceExpressionDESP;

    private DateTime MoCapRecordStartTime = DateTime.Now;
    private readonly List<MoCapBodyFrame> BodyFrames = new List<MoCapBodyFrame>();
    private readonly List<MoCapFaceFrame> FaceFrames = new List<MoCapFaceFrame>();
    private volatile bool IsRecording;

    public void StartMoCap()
    {
        if (IsRecording)
            return;

        MoCapRecordStartTime = DateTime.Now;
        BodyFrames.Clear();
        FaceFrames.Clear();

        IsRecording = true;
    }

    public void StopMoCap()
    {
        if (!IsRecording)
            return;

        IsRecording = false;
    }

    public void SaveMoCap(string filename = "MoCap.mocap")
    {
        var Data = new MoCapData();
        Data.BodyFrames = BodyFrames.ToArray();
        Data.FaceFrames = FaceFrames.ToArray();
        MoCapData.Serialize(ref Data, filename);
    }

    private void BodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
    {
        using (var frame = e.FrameReference.AcquireFrame())
        {
            if (frame != null)
            {
                if (Bodies == null)
                    Bodies = new Body[frame.BodyCount];

                frame.GetAndRefreshBodyData(Bodies);

                foreach (var body in Bodies)
                    if (body.IsTracked)
                    {
                        HighDefinitionFaceFrameSource.TrackingId = body.TrackingId;

                        var command = new CommandMessage
                        {
                            CommandType = CommandType.KinectBody,
                            KinectBody =
                                new MoCapBodyFrame
                                {
                                    SkeletonTransforms = new TransformTime[(int) MoCapKinectBone.Count]
                                }
                        };

                        foreach (JointType jointType in Enum.GetValues(typeof (JointType)))
                        {
                            Joint joint;

                            if (body.Joints.TryGetValue(jointType, out joint))
                                if (joint.TrackingState != TrackingState.NotTracked)
                                {
                                    JointOrientation jointOrientation;

                                    if (body.JointOrientations.TryGetValue(jointType, out jointOrientation))
                                    {
                                        var time = (float) (DateTime.Now - MoCapRecordStartTime).TotalSeconds;

                                        //var positionX = joint.Position.X;
                                        //var positionY = joint.Position.Y;
                                        //var positionZ = joint.Position.Z;

                                        var rotationX = jointOrientation.Orientation.X;
                                        var rotationY = jointOrientation.Orientation.Y;
                                        var rotationZ = jointOrientation.Orientation.Z;
                                        var rotationW = jointOrientation.Orientation.W;

                                        var transform = new TransformTime();
                                        transform.Time = time;
                                        transform.Position = Vector3.Zero;
                                        transform.Rotation = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
                                        transform.Scale = Vector3.One;

                                        transform.Rotation = BodyDESP[(int) jointType].Predict(transform.Rotation);

                                        command.KinectBody.SkeletonTransforms[(int) jointType] = transform;
                                    }
                                }
                        }

                        if (IsRecording)
                            BodyFrames.Add(command.KinectBody);

                        BeginWriteCommand(command);

                        break;
                    }
            }
        }
    }

    private void HighDefinitionFaceFrameReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
    {
        using (var frame = e.FrameReference.AcquireFrame())
        {
            if (frame != null && frame.IsTrackingIdValid && frame.IsFaceTracked &&
                frame.FaceAlignmentQuality == FaceAlignmentQuality.High)
            {
                frame.GetAndRefreshFaceAlignmentResult(FaceAlignment);

                var command = new CommandMessage
                {
                    CommandType = CommandType.KinectFace,
                    KinectFace =
                        new MoCapFaceFrame
                        {
                            ExpressionWeights = new float[(int) MoCapKinectFacialExpression.Count]
                        }
                };

                var time = (float) (DateTime.Now - MoCapRecordStartTime).TotalSeconds;

                var rotationX = FaceAlignment.FaceOrientation.X;
                var rotationY = FaceAlignment.FaceOrientation.Y;
                var rotationZ = FaceAlignment.FaceOrientation.Z;
                var rotationW = FaceAlignment.FaceOrientation.W;

                var transform = new TransformTime();
                transform.Time = time;
                transform.Position = Vector3.Zero;
                transform.Rotation = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
                transform.Scale = Vector3.One;

                transform.Rotation = FaceDESP.Predict(transform.Rotation);

                command.KinectFace.FaceTransform = transform;

                foreach (FaceShapeAnimations faceShapeAnimation in Enum.GetValues(typeof (FaceShapeAnimations)))
                {
                    float weight;

                    if (FaceAlignment.AnimationUnits.TryGetValue(faceShapeAnimation, out weight))
                    {
                        FaceExpressionDESP[(int) faceShapeAnimation].Update(weight);

                        command.KinectFace.ExpressionWeights[(int) faceShapeAnimation] =
                            FaceExpressionDESP[(int) faceShapeAnimation].Predict(1);
                    }
                }

                if (IsRecording)
                    FaceFrames.Add(command.KinectFace);

                BeginWriteCommand(command);
            }
        }
    }

    #endregion
}