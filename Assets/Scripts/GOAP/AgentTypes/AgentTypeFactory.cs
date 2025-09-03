using GOAP.Capabilities;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace GOAP.AgentTypes
{
    public class AgentTypeFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
            var factory = new AgentTypeBuilder("ScriptAgent");
            
            factory.AddCapability<ObjectiveCapabilityFactory>();
            factory.AddCapability<ShootCapabilityFactory>();

            return factory.Build();
        }
    }
}