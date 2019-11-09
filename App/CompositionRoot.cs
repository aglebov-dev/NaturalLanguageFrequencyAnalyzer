using App.Logic;
using Autofac;
using Contracts;
using TextProvider;

namespace App
{
    internal static class CompositionRoot
    {
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<FilePathService>();
            builder.RegisterType<Processor>();
            builder.RegisterType<TextDataProvider>().As<ITextProvider>();

            return builder.Build();
        }
    }
}
