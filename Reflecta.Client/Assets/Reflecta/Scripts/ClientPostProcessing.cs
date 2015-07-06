#region Using

using UnityEngine;

#endregion

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof (Camera))]
    [AddComponentMenu("Image Effects/Reflecta/Client Post Processing")]
    public class ClientPostProcessing : PostEffectsBase
    {
        public Shader clientPostProcessingShader;
        private Material clientPostProcessingMaterial;

        public override bool CheckResources()
        {
            CheckSupport(false, false);

            clientPostProcessingMaterial = CheckShaderAndCreateMaterial(clientPostProcessingShader,
                clientPostProcessingMaterial);

            if (!isSupported)
                ReportAutoDisable();

            return isSupported;
        }

        private new void Start()
        {
            clientPostProcessingShader = Shader.Find("Reflecta/Client Post Processing");
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            Graphics.Blit(source, destination, clientPostProcessingMaterial);
        }
    }
}