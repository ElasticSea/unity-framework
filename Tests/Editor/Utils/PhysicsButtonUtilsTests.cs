using System.Diagnostics;
using System.IO;
using System.Linq;
using ElasticSea.Framework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.UnityConverters.Math;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

namespace Tests.Editor.Gizmos
{
    public class UtilsGetPagesTests
    {
        [Test]
        public static void GetPagesTest2Elements()
        {
            var elements = new[] { ("0", 0.3f), ("1", 0.3f) };
            var maxSize = 1.5f;
            var navSize = 0.3f;
            var pages = Utils.CalculatePages(elements, maxSize, navSize, navSize);

            var expected = new[]
            {
                new[]
                {
                    new PageElement<string>
                    {
                        Element = "0",
                        Type = ElementType.Element
                    },
                    new PageElement<string>
                    {
                        Element = "1",
                        Type = ElementType.Element
                    }
                }
            };
            
            PagesEqual(expected, pages);
        }
        
        [Test]
        public static void GetPagesSmall()
        {
            var elements = new[] { ("0", 0.3f), ("1", 0.3f), ("2", 0.3f), ("3", 0.3f), ("4", 0.3f) };
            var maxSize = 0.1f;
            var navSize = 0.3f;
            var pages = Utils.CalculatePages(elements, maxSize, navSize, navSize);

            var expected = new[]
            {
                new[]
                {
                    new PageElement<string>
                    {
                        Element = "0",
                        Type = ElementType.Element
                    },
                    new PageElement<string>
                    {
                        Element = "1",
                        Type = ElementType.Element
                    },
                    new PageElement<string>
                    {
                        Element = null,
                        Type = ElementType.RightNav
                    }
                },
                new[]
                {
                    new PageElement<string>
                    {
                        Element = null,
                        Type = ElementType.LeftNav
                    },
                    new PageElement<string>
                    {
                        Element = "2",
                        Type = ElementType.Element
                    },
                    new PageElement<string>
                    {
                        Element = null,
                        Type = ElementType.RightNav
                    }
                },
                new[]
                {
                    new PageElement<string>
                    {
                        Element = null,
                        Type = ElementType.LeftNav
                    },
                    new PageElement<string>
                    {
                        Element = "3",
                        Type = ElementType.Element
                    },
                    new PageElement<string>
                    {
                        Element = "4",
                        Type = ElementType.Element
                    }
                }
            };
            
            PagesEqual(expected, pages);
        }
        
        [Test]
        public static void GetPages4ElementsTooBig()
        {
            var elements = new[] { ("0", 146.9f), ("1", 146.9f), ("2", 146.9f), ("3", 146.9f)};
            var maxSize = 500f;
            var navSize = 13.96f;
            var pages = Utils.CalculatePages(elements, maxSize, navSize, navSize);

            var expected = new[]
            {
                new[]
                {
                    new PageElement<string>
                    {
                        Element = "0",
                        Type = ElementType.Element
                    },
                    new PageElement<string>
                    {
                        Element = "1",
                        Type = ElementType.Element
                    },
                    new PageElement<string>
                    {
                        Element = "2",
                        Type = ElementType.Element
                    },
                    new PageElement<string>
                    {
                        Element = null,
                        Type = ElementType.RightNav
                    }
                },
                new[]
                {
                    new PageElement<string>
                    {
                        Element = null,
                        Type = ElementType.LeftNav
                    },
                    new PageElement<string>
                    {
                        Element = "3",
                        Type = ElementType.Element
                    }
                }
            };
            
            PagesEqual(expected, pages);
        }
        
        // [Test]
        // public static void GetPagesTestPageTooSmall()
        // {
        //     var elements = new[] { "0", "1" };
        //     var maxSize = 0f;
        //     var navSize = 0.3f;
        //     var buttonSizeFormatter = new TestFormater(0.3f);
        //     var pages = PhysicsButtonUtils.GetPages(elements, maxSize, navSize, buttonSizeFormatter);
        //
        //     var expected = new[]
        //     {
        //         new[]
        //         {
        //             new PageElement<string>
        //             {
        //                 Element = "0",
        //                 Type = ElementType.Element
        //             },
        //             new PageElement<string>
        //             {
        //                 Type = ElementType.RightNav
        //             }
        //         }
        //     };
        //     
        //     PagesEqual(expected, pages);
        // }
        
        [Test]
        public static void GetPagesTest4Elements3MaxInPage()
        {
            var elements = new[] { ("0", 0.5f), ("1", 0.5f), ("2", 0.5f), ("3", 0.5f) };
            var maxSize = 1.5f;
            var navSize = 0.5f;
            var pages = Utils.CalculatePages(elements, maxSize, navSize, navSize);

            var expected = new[]
            {
                new[]
                {
                    new PageElement<string>()
                    {
                        Element = "0",
                        Type = ElementType.Element
                    },
                    new PageElement<string>()
                    {
                        Element = "1",
                        Type = ElementType.Element
                    },
                    new PageElement<string>()
                    {
                        Type = ElementType.RightNav
                    }
                },
                new[]
                {
                    new PageElement<string>
                    {
                        Type = ElementType.LeftNav
                    },
                    new PageElement<string>
                    {
                        Element = "2",
                        Type = ElementType.Element
                    },
                    new PageElement<string>
                    {
                        Element = "3",
                        Type = ElementType.Element
                    }
                }
            };
            
            PagesEqual(expected, pages);
        }

        // [Test]
        // public static void GetPagesTest()
        // {
        //     var elements = Enumerable.Range(0, 11).Select(i => i.ToString()).ToArray();
        //     var maxSize = 1.5f;
        //     var navSize = 0.3f;
        //     var buttonSizeFormatter = new TestFormater(0.3f);
        //     var pages = PhysicsButtonUtils.GetPages(elements, maxSize, navSize, buttonSizeFormatter);
        //
        //     // var expected = new PageElement<string>[][]
        //     // {
        //     //     new PageElement<string>[]
        //     //     {
        //     //         
        //     //     }
        //     // }
        //     // Assert.AreEqual(pages.Length, 3);
        //     // Assert.AreEqual(pages[0].Length, 5);
        //     // Assert.AreEqual(pages[1].Length, 5);
        //     // Assert.AreEqual(pages[2].Length, 5);
        //     // Debug.Log("hhelo");
        //     // PagesEqual()
        // }

        private static void PagesEqual(PageElement<string>[][] expected, PageElement<string>[][] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var e = expected[i];
                var a = actual[i];
                
                Assert.AreEqual(e.Length, a.Length);
                for (int j = 0; j < e.Length; j++)
                {
                    var e1 = e[j];
                    var a1 = a[j];
                    
                    Assert.AreEqual(e1.Type, a1.Type);
                    Assert.AreEqual(e1.Element, a1.Element);
                }
            }
        }
    }
}