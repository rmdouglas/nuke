﻿// Copyright Sebastian Karasek, Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/azure-keyvault/blob/master/LICENSE

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Nuke.Common.Execution;

namespace Nuke.Common.Tools.AzureKeyVault.Attributes
{
    /// <summary> Attribute to obtain a key from from the Azure KeyVault defined by <see cref="KeyVaultSettingsAttribute"/>.</summary>
    [PublicAPI]
    public class KeyVaultKeyAttribute : KeyVaultSecretAttribute
    {
        /// <summary> Attribute to obtain a key from from the Azure KeyVault defined by <see cref="KeyVaultSettingsAttribute"/>.</summary>
        /// <param name="keyName">The name of the key to obtain.</param>
        public KeyVaultKeyAttribute (string keyName = null)
                : base(keyName)
        {
        }

        public override object GetValue (MemberInfo member, object instance)
        {
            if (member.GetMemberType() != typeof(KeyVaultKey))
                throw new NotSupportedException();
            return base.GetValue(member, instance);
        }
    }
}
