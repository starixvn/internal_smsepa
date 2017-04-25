namespace SMS.EPA.Site
{
    using BundleTransformer.Core.Orderers;
    using BundleTransformer.Core.Transformers;
    using System.Web.Optimization;

    public static class BundleEntensions
    {
        public static void AddAndInclude(this BundleCollection bundles, string bundleVirtualPath, params string[] virtualPaths)
        {
            bundles?.Add(new Bundle(bundleVirtualPath)
            {
                Transforms = { new CssTransformer(), new CssMinify() },
                Orderer = new NullOrderer()
            }.Include(virtualPaths));
        }
    }
}