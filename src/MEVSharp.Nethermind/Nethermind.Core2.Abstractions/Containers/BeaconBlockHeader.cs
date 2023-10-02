﻿//  Copyright (c) 2018 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.

using System;
using Nethermind.Core2.Crypto;
using Nethermind.Core2.Types;
using Nethermind.HashLib;
namespace Nethermind.Core2.Containers
{
    public class BeaconBlockHeader : IEquatable<BeaconBlockHeader>
    {
        public static readonly BeaconBlockHeader Zero =
            new BeaconBlockHeader(Slot.Zero, ValidatorIndex.Zero, Root.Zero, Root.Zero, Root.Zero);

        public BeaconBlockHeader(
            Slot slot,
            ValidatorIndex validatorIndex,
            Root parentRoot,
            Root stateRoot,
            Root bodyRoot)
        {
            Slot = slot;
            ValidatorIndex = validatorIndex;
            ParentRoot = parentRoot;
            StateRoot = stateRoot;
            BodyRoot = bodyRoot;
        }

        public BeaconBlockHeader()
            : this(Slot.Zero, ValidatorIndex.Zero, Root.Zero, Root.Zero, Root.Zero)
        {
        }

        public Root BodyRoot { get; set; }
        public Root ParentRoot { get; private set; }
        public Slot Slot { get; private set; }
        public ValidatorIndex ValidatorIndex { get; private set; }
        public Root StateRoot { get; private set; }

        public void SetStateRoot(Root stateRoot)
        {
            StateRoot = stateRoot;
        }

        public bool Equals(BeaconBlockHeader other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }
            
            return BodyRoot.Equals(other.BodyRoot)
                && ParentRoot.Equals(other.ParentRoot)
                && Slot == other.Slot
                && StateRoot.Equals(other.StateRoot);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BodyRoot, ParentRoot, Slot, StateRoot);
        }

        public override bool Equals(object? obj)
        {
            var other = obj as BeaconBlockHeader;
            return !(other is null) && Equals(other);
        }

        public override string ToString()
        {
            return $"s={Slot}_p={ParentRoot.ToString().Substring(0, 10)}_st={StateRoot.ToString().Substring(0, 10)}_bd={BodyRoot.ToString().Substring(0, 10)}";
        }
    }
}
