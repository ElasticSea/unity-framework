using System;
using System.Collections.Generic;
using System.Linq;
using Core.Ui.Binding;
using Core.Ui.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.AutoBind
{
    public class AutoBindUiItem : MonoBehaviour
    {
        [SerializeField] private Text Label;
        [SerializeField] private Transform payloadContainer;
        [SerializeField] private Transform firstPart;
        [SerializeField] private Transform secondPart;

        [Header("Controls")]
        [SerializeField] private GameObject textBox; //TextBox
        [SerializeField] private GameObject toggle; //SlideToggle
        [SerializeField] private GameObject listView; //ListView
        [SerializeField] private GameObject vector2Field; //ListView
        [SerializeField] private GameObject combobox;
        
        // [SerializeField] private GameObject combobox; //Combobox
        // [SerializeField] private GameObject pathField; //PathField
        // [SerializeField] private GameObject listEditor; //PathField
        // [SerializeField] private GameObject rangeField; //PathField

        public void SetItem(IAutoBindItem autoBindItem)
        {
            Label.text = autoBindItem.Name;

            // if (autoBindItem.Expandable == false)
            // {
                var attributeType = autoBindItem.GetType().GetGenericArguments()[0];

                if (autoBindItem.ControlType == ControlType.EditList)
                {
                    // TODO this just does not scale
                    // if (typeof(IEnumerable<Uri>).IsAssignableFrom(attributeType))
                    // {
                    //     SetOrientation(false);
                    //     var instantiateX = Instantiate(listEditor);
                    //     var lv = instantiateX.GetComponentInChildren<ListEditor>();
                    //     var property2X = new PropertyBinding<IEnumerable<Uri>>(() => configItem.Value as IEnumerable<Uri>,
                    //         s => configItem.Value = s);
                    //     property2X.BindProperty(lv.As<Uri>());
                    //
                    //     lv.As<Uri>().CustomFactory = () =>
                    //     {
                    //         var lolo = Instantiate(textBox);
                    //         var ib = lolo.GetComponentInChildren<InputBox>();
                    //         ib.AddValidator(new CustomValidator{Validator = s =>
                    //             {
                    //                 var valid = Uri.IsWellFormedUriString(s, UriKind.Absolute);
                    //                 return new ValidationResult{IsValid = valid};
                    //             }
                    //         });
                    //         var adapter = new BindingAdapter<string, Uri>(ib, s => new Uri(s), uri => uri.ToString());
                    //
                    //         return (lolo.GetComponent<Component>(), adapter);
                    //     };
                    //     
                    //     AddToContainer(instantiateX);
                    // }
                }else if (autoBindItem.Values.Any())
                {
                    switch (autoBindItem.ControlType)
                    {
                        case ControlType.Combobox:
                            var instantiate = Instantiate(combobox);
                            var cb = instantiate.GetComponentInChildren<Combobox>();
                            var property = new PropertyBinding<object>(() => autoBindItem.Value as object, s => autoBindItem.Value = s);
                            var property2 = new PropertyBinding<IEnumerable<object>>(() => autoBindItem.Values.Cast<object>().ToList(),
                                s => autoBindItem.Value = s);
                            property2.BindProperty(cb.List);
                            property.BindProperty(cb.Selected);
                            AddToContainer(instantiate);
                            break;
                        case ControlType.Switchbox:
                            var instantiateX = Instantiate(listView);
                            var lv = instantiateX.GetComponentInChildren<ListView>();
                            var propertyX = new PropertyBinding<object>(() => autoBindItem.Value as object, s => autoBindItem.Value = s);
                            var property2X = new PropertyBinding<IEnumerable<object>>(() => autoBindItem.Values.Cast<object>().ToList(),
                                s => autoBindItem.Value = s);
                            property2X.BindProperty(lv.As<object>().List);
                            propertyX.BindProperty(lv.As<object>().Value);
                            AddToContainer(instantiateX);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else if (attributeType == typeof(string))
                {
                    // if (configItem.OptionsControl == OptionsControlType.FileExplorer)
                    // {
                    //     var instantiate = Instantiate(pathField);
                    //     var pf = instantiate.GetComponentInChildren<PathField>();
                    //     pf.Path = Application.streamingAssetsPath;
                    //     var property = new PropertyBinding<string>(() => configItem.Value as string, s => configItem.Value = s);
                    //     property.BindProperty(pf);
                    //     AddToContainer(instantiate); 
                    // }
                    // else
                    // {
                        var instantiate = Instantiate(textBox);
                        var tb = instantiate.GetComponentInChildren<InputBox>();
                        var property = new PropertyBinding<string>(() => autoBindItem.Value as string, s => autoBindItem.Value = s);
                        property.BindProperty(tb);
                        AddToContainer(instantiate);
                    // }
                }
                // else if (attributeType == typeof(Range))
                // {
                //     var instantiate = Instantiate(rangeField);
                //     var rf = instantiate.GetComponentInChildren<RangeField>();
                //     var property = new PropertyBinding<Range>(() => configItem.Value as Range, s => configItem.Value = s);
                //     property.BindProperty(rf);
                //
                //     if (configItem.MinMaxLimit != null)
                //     {
                //         rf.MinLimit = configItem.MinMaxLimit.Min;
                //         rf.MaxLimit = configItem.MinMaxLimit.Max;
                //     }
                //     else{
                //         rf.MinLimit = float.MinValue;
                //         rf.MaxLimit = float.MaxValue;
                //     }
                //     AddToContainer(instantiate); 
                // }
                else if (attributeType == typeof(Uri))
                {
                    var instantiate = Instantiate(textBox);
                    var tb = instantiate.GetComponentInChildren<InputBox>();
                    var property = new PropertyBinding<string>(() => (autoBindItem.Value as Uri)?.AbsoluteUri, s => autoBindItem.Value = new Uri(s));
                    property.BindProperty(tb);
                    AddToContainer(instantiate);
                }
                else if (attributeType == typeof(float))
                {
                    var instantiate = Instantiate(textBox);
                    var tb = instantiate.GetComponentInChildren<InputBox>().AsFloat();
                    var property = new PropertyBinding<float>(() => autoBindItem.Value is float ? (float) autoBindItem.Value : 0,
                        s => autoBindItem.Value = s);
                    property.BindProperty(tb);
                    AddToContainer(instantiate);
                }
                else if (attributeType == typeof(int))
                {
                    var instantiate = Instantiate(textBox);
                    var tb = instantiate.GetComponentInChildren<InputBox>().AsInt();
                    var property = new PropertyBinding<int>(() => autoBindItem.Value is int ? (int) autoBindItem.Value : 0,
                        s => autoBindItem.Value = s);
                    property.BindProperty(tb);
                    AddToContainer(instantiate);
                }
                else if (attributeType == typeof(bool))
                {
                    var instantiate = Instantiate(toggle);
                    var tgl =
                        instantiate.GetComponentsInChildren<Component>()
                            .FirstOrDefault(c => c is IUiBinding<bool>) as IUiBinding<bool>;
                    var property = new PropertyBinding<bool>(() => autoBindItem.Value is bool ? (bool) autoBindItem.Value : false,
                        s => autoBindItem.Value = s);
                    property.BindProperty(tgl);
                    AddToContainer(instantiate);
                }else if (attributeType == typeof(Vector2))
                {
                    
                    var instantiate = Instantiate(vector2Field);
                    var tgl =
                        instantiate.GetComponentsInChildren<Component>()
                            .FirstOrDefault(c => c is IUiBinding<Vector2>) as IUiBinding<Vector2>;
                    var property = new PropertyBinding<Vector2>(() => autoBindItem.Value as Vector2? ?? default,
                        s => autoBindItem.Value = s);
                    property.BindProperty(tgl);
                    AddToContainer(instantiate);
                }
            // }
        }

        private void SetOrientation(bool isHorizontal)
        {
            DestroyImmediate(GetComponent<LayoutGroup>());
            DestroyImmediate(GetComponent<LayoutElement>());
                
            if (isHorizontal)
            {
                var layout = gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.childControlWidth = true;
                layout.childControlHeight = true;
                layout.childForceExpandWidth = true;
                layout.childForceExpandHeight = true;

                layout.padding.left = 25;
                layout.padding.right = 25;
                
                var le = firstPart.GetComponent<LayoutElement>();
                le.preferredWidth = 0;
                le.flexibleWidth = 1;
                
                var le2 = secondPart.GetComponent<LayoutElement>();
                le2.preferredWidth = 0;
                le2.flexibleWidth = 2;

                var lala = gameObject.AddComponent<LayoutElement>();
                lala.preferredHeight = 72;
            }
            else
            {
                var lala = gameObject.AddComponent<ContentSizeFitter>();
                lala.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                
                var layout = gameObject.AddComponent<VerticalLayoutGroup>();
                layout.childControlWidth = true;
                layout.childControlHeight = true;
                layout.childForceExpandWidth = true;
                layout.childForceExpandHeight = false;
                
                layout.padding.left = 25;
                layout.padding.right = 25;
                
                var le = firstPart.GetComponent<LayoutElement>();
                le.preferredHeight = 72;
                
                var le2 = secondPart.GetComponent<LayoutElement>();
                le2.preferredHeight = 300;
            }
        }

        private void AddToContainer(GameObject instance)
        {
            var rt = instance.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            instance.transform.SetParent(payloadContainer, false);
        }
    }
}