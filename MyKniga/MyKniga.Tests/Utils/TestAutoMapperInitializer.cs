namespace MyKniga.Tests.Utils
{
    using AutoMapper;
    using Common.Mapping;

    public static class TestAutoMapperInitializer
    {
        private static bool isInitialized;

        public static void InitializeAutoMapper()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;

            Mapper.Initialize(config => config.AddProfile<DefaultProfile>());
        }
    }
}