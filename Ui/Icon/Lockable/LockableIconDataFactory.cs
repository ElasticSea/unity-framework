using System;

namespace ElasticSea.Framework.Ui.Icon.Lockable
{
    public class LockableIconDataFactory
    {
        public string Id;
        public Func<LockableIconData> Factory;
    }
}