// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Tera.Game.Messages
{
    public class EachSkillResultServerMessage : ParsedMessage
    {
        public EntityId Source { get; private set; }
        public EntityId Target { get; private set; }
        public int Amount { get; private set; }
        public int SkillId { get; private set; }
        public SkillResultFlags Flags { get; private set; }
        public bool IsCritical { get; private set; }
        public bool IsHeal => (Flags & SkillResultFlags.Heal) != 0;
        public bool IsResolve => (Flags & SkillResultFlags.Resolve) != 0;
        public bool IsUseless => Flags == 0 || IsResolve;

        internal EachSkillResultServerMessage(TeraMessageReader reader)
            : base(reader)
        {
            reader.Skip(4);
            Source = reader.ReadEntityId();
            Target = reader.ReadEntityId();
            reader.Skip(4);
            SkillId = reader.ReadInt32() & 0x3FFFFFF;
            reader.Skip(16);
            Amount = reader.ReadInt32();
            Flags = (SkillResultFlags)reader.ReadInt32();
            IsCritical = (reader.ReadByte() & 1) != 0;
        }

        [Flags]
        public enum SkillResultFlags : int
        {
            Bit0 = 1,// Usually 1 for attacks, 0 for blocks/dodges but I don't understand its exact semantics yet
            Heal = 2,
            Resolve = 4,//dfa resolve has this flag set, nothing else seems to, this is just an assumption for now
            Bit16 = 0x10000,
            Bit18 = 0x40000
        }
    }
}
