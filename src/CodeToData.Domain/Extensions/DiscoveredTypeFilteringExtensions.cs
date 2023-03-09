using System;
using System.Collections.Generic;
using System.Linq;
using CodeToData.Domain.Models;

namespace CodeToData.Domain.Extensions
{
    public static class DiscoveredTypeFilteringExtensions
    {
        public static IEnumerable<DiscoveredType> FilterBySourceNamespace(this IEnumerable<DiscoveredType> discoveredTypes,
            string filter, bool caseInsensitive = true)
        {
            return discoveredTypes
                .Where(discoveredType =>
                    discoveredType.SourceNamespace.Contains(filter, ComparisonType(caseInsensitive)));
        }

        public static IEnumerable<DiscoveredType> FilterBySourceAssembly(this IEnumerable<DiscoveredType> discoveredTypes,
            string filter, bool caseInsensitive = true)
        {
            return discoveredTypes
                .Where(discoveredType =>
                    discoveredType.SourceAssembly.Contains(filter, ComparisonType(caseInsensitive)));
        }

        public static IEnumerable<DiscoveredType> InvertFilterBySourceAssembly(this IEnumerable<DiscoveredType> discoveredTypes,
            string filter, bool caseInsensitive = true)
        {
            return discoveredTypes
                .Where(discoveredType =>
                    !discoveredType.SourceAssembly.Contains(filter, ComparisonType(caseInsensitive)));
        }

        public static IEnumerable<DiscoveredType> InvertFilterBySourceNamespace(this IEnumerable<DiscoveredType> discoveredTypes,
            string filter, bool caseInsensitive = true)
        {
            return discoveredTypes
                .Where(discoveredType =>
                    !discoveredType.SourceNamespace.Contains(filter, ComparisonType(caseInsensitive)));
        }

        private static StringComparison ComparisonType(bool caseInsensitive)
        {
            return caseInsensitive ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
        }
    }
}