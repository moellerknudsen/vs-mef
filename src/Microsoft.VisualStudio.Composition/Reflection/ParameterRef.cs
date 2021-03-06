﻿// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.VisualStudio.Composition.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ParameterRef : IEquatable<ParameterRef>
    {
        /// <summary>
        /// Gets the string to display in the debugger watch window for this value.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{this.DeclaringType.FullName}.{this.Method.DebuggerDisplay}(p-index: {this.ParameterIndex})";

        /// <summary>
        /// A cache behind the <see cref="ParameterInfo"/> property.
        /// </summary>
        private ParameterInfo cachedParameterInfo;

        public ParameterRef(MethodRef method, int parameterIndex)
        {
            Requires.NotNull(method, nameof(method));
            Requires.Range(parameterIndex >= 0, nameof(parameterIndex));

            this.Method = method;
            this.ParameterIndex = parameterIndex;
        }

        public ParameterRef(ParameterInfo parameterInfo, Resolver resolver)
        {
            Requires.NotNull(parameterInfo, nameof(parameterInfo));
            Requires.NotNull(resolver, nameof(resolver));

            this.Method = new MethodRef((MethodBase)parameterInfo.Member, resolver);
            this.ParameterIndex = parameterInfo.Position;
        }

        public MethodRef Method { get; }

        public ParameterInfo ParameterInfo => this.cachedParameterInfo ?? (this.cachedParameterInfo = this.Resolve());

        public TypeRef DeclaringType => this.Method.DeclaringType;

        public int MethodMetadataToken => this.Method.MetadataToken;

        /// <summary>
        /// Gets a 0-based index describing which parameter in the method this references.
        /// </summary>
        public int ParameterIndex { get; }

        public AssemblyName AssemblyName => this.DeclaringType.AssemblyName;

        internal Resolver Resolver => this.DeclaringType.Resolver;

        public static ParameterRef Get(ParameterInfo parameter, Resolver resolver)
        {
            if (parameter != null)
            {
                return new ParameterRef(parameter, resolver);
            }

            return default(ParameterRef);
        }

        public bool Equals(ParameterRef other)
        {
            return this.Method.Equals(other.Method)
                && this.ParameterIndex == other.ParameterIndex;
        }

        public override int GetHashCode() => unchecked(this.Method.MetadataToken + this.ParameterIndex);

        public override bool Equals(object obj) => obj is ParameterRef parameter && this.Equals(parameter);

        internal void GetInputAssemblies(ISet<AssemblyName> assemblies)
        {
            Requires.NotNull(assemblies, nameof(assemblies));

            this.DeclaringType.GetInputAssemblies(assemblies);
        }
    }
}
