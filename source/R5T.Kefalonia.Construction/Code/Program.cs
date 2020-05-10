using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using R5T.Liverpool;


namespace R5T.Kefalonia.Construction
{
    class Program : AsyncHostedServiceProgramBase
    {
        static async Task Main(string[] args)
        {
            await HostedServiceProgram.RunAsync<Program, Startup>();
        }


        public Program(IApplicationLifetime applicationLifetime)
            : base(applicationLifetime)
        {
        }

        protected override Task SubMainAsync()
        {
            Console.WriteLine("Hello world!");

            return Task.CompletedTask;
        }
    }
}
