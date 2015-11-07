using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Protocol.Game.Messages
{
    public class SpawnMeServerMessage: ParsedMessage
    {
        public EntityId Id { get; private set; }

        internal SpawnMeServerMessage(TeraMessageReader reader)
            : base(reader)
        {
            Id = reader.ReadEntityId();
        }
    }
}
