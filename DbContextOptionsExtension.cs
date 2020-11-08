using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

namespace Question
{
    public class DbContextOptionsExtension : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo? _info;
        
        public void Validate(IDbContextOptions options)
        {
        }

        public DbContextOptionsExtensionInfo Info {
            get
            {
                return this._info ??= (MyDbContextOptionsExtensionInfo)new MyDbContextOptionsExtensionInfo((IDbContextOptionsExtension)this);
            }
        }

        void IDbContextOptionsExtension.ApplyServices(IServiceCollection services)
        {
            services.AddSingleton<IMethodCallTranslatorProvider, MethodCallTranslatorPlugin>();
        }

        private sealed class MyDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
        {
            public MyDbContextOptionsExtensionInfo(IDbContextOptionsExtension instance) : base(instance) { }
            
            public override bool IsDatabaseProvider => false;
            
            public override string LogFragment => "";

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                
            }

            public override long GetServiceProviderHashCode()
            {
                return 0;
            }
        }
    }
}