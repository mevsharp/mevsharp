using Lantern;
using Nethermind.Core.Crypto;
using Nethermind.Core2.Containers;
using Nethermind.Core2.Crypto;
using Nethermind.Core2.Types;
using SszSharp;
using Root = Nethermind.Core2.Crypto.Root;
namespace MEVSharp.Nethermind.Crypto
{
    public class LightClientUtility
    {

        public BLSUtility crypto = new BLSUtility();
        public Domain ComputeDomain(DomainType domainType, ForkVersion forkVersion, Root genesisValidatorsRoot)
        {
            Root forkDataRoot = new ForkData(forkVersion, genesisValidatorsRoot).HashTreeRoot();
            byte[] array2 = domainType.AsSpan().Slice(0, 4).ToArray();
            byte[] array3 = forkDataRoot.AsSpan().Slice(0, 28).ToArray();
            int number = 0;
            for (int i = 0; i < array3.Length; i++)
            {
                number++;
            }
            byte[] array = array2.Concat(array3).ToArray();
            return new Domain(array);
        }

        public Root ComputeSigningRoot(Root blockRoot, Domain domain)
        {
            SigningRoot domainWrappedObject = new SigningRoot(blockRoot, domain);
            return domainWrappedObject.HashTreeRoot();
        }
       
    }
  
}
