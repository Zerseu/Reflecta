namespace Reflecta
{
    public static class Constants
    {
        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 720;
        public const int SCREEN_BYTE_COUNT = SCREEN_WIDTH*SCREEN_HEIGHT*4;
        public const int RENDERING_PIPELINE_BUFFER_SIZE = 1024*1024*4;
        public const int COMMAND_PIPELINE_BUFFER_SIZE = 1024*64;
    }
}