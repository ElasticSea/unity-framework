using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityObject = UnityEngine.Object;

namespace ElasticSea.Framework.Extensions
{
	public static class GameObjectExtensions
	{
		public static void DestroyChildren(this Transform transform, bool immediate = false)
		{
			DestroyChildren(transform.gameObject, immediate);
		}

		public static void DestroyChildren(this GameObject gameObject, bool immediate = false, bool allowDestroyingAssets = false)
		{
			var transform = gameObject.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				var child = transform.GetChild(i).gameObject;
				if (immediate)
				{
					UnityObject.DestroyImmediate(child, allowDestroyingAssets);
				}
				else
				{
					UnityObject.Destroy(child);
				}
			}
		}

		public static string GetParentName(this GameObject source)
		{
			var parent = source.transform.parent;
			return parent == null ? string.Empty : parent.name;
		}

		public static bool HasComponent(this Transform source, Type componentType)
		{
			return HasComponent(source.gameObject, componentType);
		}

		public static bool HasComponent(this GameObject source, Type componentType)
		{
			return source.GetComponent(componentType) != null;
		}

		public static bool HasComponent(this Transform source, string componentName)
		{
			return HasComponent(source.gameObject, componentName);
		}

		public static bool HasComponent(this GameObject source, string componentName)
		{
			return source.GetComponent(componentName) != null;
		}

		public static bool HasComponent<T>(this Transform source) where T : Component
		{
			return HasComponent<T>(source.gameObject);
		}

		public static bool HasComponent<T>(this GameObject source) where T : Component
		{
			return source.GetComponent<T>() != null;
		}

		/// <summary>
		/// Recursively returns children gameObjects excluding inactive ones
		/// </summary>
		public static IEnumerable<GameObject> GetChildren(this GameObject parent)
		{
			return GetChildren(parent, false);
		}

		/// <summary>
		/// Recursively returns children gameObjects
		/// </summary>
		public static IEnumerable<GameObject> GetChildren(this GameObject parent, bool includeInactive)
		{
			var transform = parent.transform;
			int count = transform.childCount;
			for (int i = 0; i < count; i++)
			{
				yield return transform.GetChild(i).gameObject;
			}
		}

		/// <summary>
		/// Considers the gameObject's parents in the name
		/// ex: if 'Child' had a parent "Parent1" and "Parent1" had a parent "Parent2"
		/// the result is "Parent2.Parent1.Child"
		/// </summary>
		public static string GetLongName(this GameObject gameObject)
		{
			var parents = gameObject.GetParents();
			return parents.IsEmpty() ? gameObject.name :
				string.Join(".", parents.Select(p => p.name)
										.Reverse()
										.ToArray()
							) + "." + gameObject.name;
		}

		/// <summary>
		/// Deactivates (calls SetActive(false)) this gameObject
		/// </summary>
		public static void Deactivate(this GameObject go)
		{
			go.SetActive(false);
		}

		/// <summary>
		/// Activates (calls SetActive(true)) this gameObject
		/// </summary>
		public static void Activate(this GameObject go)
		{
			go.SetActive(true);
		}

		/// <summary>
		/// Returns an array of all the components attached to this gameObject with the gameObject included
		/// </summary>
		public static UnityObject[] GetAllComponentsIncludingSelf(this GameObject go)
		{
			Component[] comps = go.GetAllComponents();
			int len = comps.Length;
			UnityObject[] targets = new UnityObject[len + 1];
			Array.Copy(comps, 0, targets, 1, len);
			targets[0] = go;
			return targets;
		}

		/// <summary>
		/// Destroys all children objects under this gameObject
		/// </summary>
		public static void ClearChildren(this GameObject go)
		{
			ClearChildren(go, child => true);
		}

		/// <summary>
		/// Destroys children objects under this gameObject meeting the specified predicate condition
		/// </summary>
		public static void ClearChildren(this GameObject go, Predicate<GameObject> predicate)
		{
			var all = go.GetComponentsInChildren<Transform>();
			for (int i = all.Length - 1; i > 0; i--)
			{
				var child = all[i].gameObject;
				if (predicate(child))
					UnityObject.Destroy(child);
			}
		}

		/// <summary>
		/// Destroys all components in this gameObject
		/// </summary>
		public static void ClearComponents(this GameObject go)
		{
			ClearComponents(go, c => true);
		}

		/// <summary>
		/// Destroys any component in this gameObject meeting the specified predicate
		/// </summary>
		public static void ClearComponents(this GameObject go, Predicate<Component> predicate)
		{
			var all = GetAllComponents(go);
			for (int i = all.Length - 1; i > 0; i--) // Skip Transform
			{
				var c = all[i];
				if (predicate(c))
					UnityObject.Destroy(c);
			}
		}

		/// <summary>
		/// Returns an array of all the components attached to this gameObject
		/// </summary>
		public static Component[] GetAllComponents(this GameObject go)
		{
			return go.GetComponents<Component>();
		}

		/// <summary>
		/// Returns the names of all the components attached to this gameObject
		/// </summary>
		public static string[] GetComponentsNames(this GameObject go)
		{
			return go.GetAllComponents().Select(c => c.GetType().Name).ToArray();
		}

		/// <summary>
		/// Returns an array of the parent gameObjects above this gameObject
		/// Ex: say we had the following hierarchy:
		/// GO 1
		/// --- GO 1.1
		/// --- GO 1.2
		/// ----- GO 1.2.1
		/// Then the parents of GO 1.2.1 are GO 1.2 and GO 1
		/// </summary>
		public static GameObject[] GetParents(this GameObject go)
		{
			return EnumerateParentObjects(go).ToArray();
		}

		/// <summary>
		/// Returns a lazy enumerable of the parent gameObjects above this gameObject
		/// Ex: say we had the following hierarchy:
		/// GO 1
		/// --- GO 1.1
		/// --- GO 1.2
		/// ----- GO 1.2.1
		/// Then the parents of GO 1.2.1 are GO 1.2 and GO 1
		/// </summary>
		public static IEnumerable<GameObject> EnumerateParentObjects(this GameObject go)
		{
			var currentParent = go.transform.parent;
			while (currentParent != null)
			{
				yield return currentParent.gameObject;
				currentParent = currentParent.parent;
			}
		}

		public static Component GetOrAddComponent(this GameObject go, Type componentType)
		{
			var result = go.GetComponent(componentType);
			return result == null ? go.AddComponent(componentType) : result;
		}

		public static T GetOrAddComponent<T>(this GameObject go) where T : Component
		{
			return GetOrAddComponent(go, typeof(T)) as T;
		}

		/// <summary>
		/// Gets the child gameObject whose name is specified by 'wanted'
		/// The search is non-recursive by default unless true is passed to 'recursive'
		/// </summary>
		public static GameObject GetChild(this GameObject inside, string wanted, bool recursive = false)
		{
			foreach (Transform child in inside.transform)
			{
				if (child.name == wanted)
                    return child.gameObject;

				if (recursive)
				{
					var within = GetChild(child.gameObject, wanted, true);
					if (within != null)
                        return within;
				}
			}
			return null;
		}

		/// <summary>
		/// Adds and returns a child gameObject to this gameObject with the specified name and HideFlags
		/// </summary>
		public static GameObject AddChild(this GameObject parent, string name, HideFlags flags = HideFlags.None)
		{
			return AddRelative(parent, name, relative =>
			{
				relative.parent = parent.transform;
				relative.Reset();
			}, flags);
		}

		private static GameObject AddRelative(this GameObject from, string name, Action<Transform> setParent, HideFlags flags = HideFlags.None)
		{
			var relative = new GameObject(name);
			relative.hideFlags = flags;
			setParent(relative.transform);
			return relative;
		}

		/// <summary>
		/// Gets or adds the child gameObject whose name is 'name'
		/// Pass true to 'recursive' if you want the search to be recursive
		/// Specify HideFlags if you want to add the child using those flags
		/// </summary>
		public static GameObject GetOrAddChild(this GameObject parent, string name, bool recursive = false, HideFlags flags = HideFlags.None)
		{
			var child = parent.GetChild(name, recursive);
			return child == null ? parent.AddChild(name, flags) : child;
		}

		/// <summary>
		/// Gets the gameObject child under this gameObject whose path is specified by childPath
		/// </summary>
		public static GameObject GetChildAtPath(this GameObject inside, string childPath, bool throwIfNotFound)
		{
			return GetRelativeAtPath(inside, "child", childPath, throwIfNotFound, (path, child) => child.GetChild(path));
		}

		/// <summary>
		/// Gets the gameObject child under this gameObject whose path is specified by childPath
		/// Throws an InvalidOperationException is not found
		/// </summary>
		public static GameObject GetChildAtPath(this GameObject inside, string childPath)
		{
			return GetChildAtPath(inside, childPath, true);
		}

		/// <summary>
		/// Gets or adds if not found the gameObject child under this gameObject whose path is specified by childPath
		/// </summary>
		public static GameObject GetOrAddChildAtPath(this GameObject inside, string childPath)
		{
			return GetOrAddRelativeAtPath(inside, childPath,
				(path, parent) => parent.GetChild(path),
				(path, parent) => parent.AddChild(path)
			);
		}

		/// <summary>
		/// Adds and returns a parent gameObject to this gameObject with the specified name and HideFlags
		/// </summary>
		public static GameObject AddParent(this GameObject child, string name, HideFlags flags = HideFlags.None)
		{
			return AddRelative(child, name, relative =>
			{
				child.transform.parent = relative;
				child.transform.Reset();
			}, flags);
		}

		/// <summary>
		/// Gets the parent whose name is wanted above this gameObject
		/// </summary>
		public static GameObject GetParent(this GameObject child, string wanted)
		{
			Transform currentParent = child.transform.parent;
			while (currentParent != null)
			{
				if (currentParent.name == wanted)
					return currentParent.gameObject;
				currentParent = currentParent.parent;
			}
			return null;
		}

		/// <summary>
		/// Gets or add the specified parent to this gameObject
		/// </summary>
		public static GameObject GetOrAddParent(this GameObject child, string name, HideFlags flags = HideFlags.None)
		{
			var parent = child.GetParent(name);
			return parent == null ? child.AddParent(name, flags) : parent;
		}

		/// <summary>
		/// Gets the parent of this gameObject whose path is specified by parentPath
		/// </summary>
		public static GameObject GetParentAtPath(this GameObject from, string parentPath, bool throwIfNotFound)
		{
			return GetRelativeAtPath(from, "parent", parentPath, throwIfNotFound, (path, parent) => parent.GetParent(path));
		}

		/// <summary>
		/// Gets the parent of this gameObject whose path is specified by parentPath
		/// Throws an InvalidOperationException if not found
		/// </summary>
		public static GameObject GetParentAtPath(this GameObject from, string parentPath)
		{
			return GetParentAtPath(from, parentPath, true);
		}

		/// <summary>
		/// Gets or adds the parent to this gameObject whose path is specifeid by parentPath
		/// </summary>
		/// <param name="from"></param>
		/// <param name="parentPath"></param>
		/// <returns></returns>
		public static GameObject GetOrAddParentAtPath(this GameObject from, string parentPath)
		{
			return GetOrAddRelativeAtPath(from, parentPath,
				(path, child) => child.GetParent(path),
				(path, child) => child.AddParent(path)
			);
		}

		private static GameObject GetOrAddRelativeAtPath(this GameObject from, string relativePath,
			Func<string, GameObject, GameObject> get,
			Func<string, GameObject, GameObject> add)
		{
			return OperateOnRelativeAtPath(from, relativePath, (operatingName, previous, current) =>
			{
				if (current == null)
					current = add(operatingName, previous);
				return current;
			}, get);
		}

		private static GameObject GetRelativeAtPath(this GameObject from,
			string relative, string relativePath,
			bool throwIfNotFound, Func<string, GameObject, GameObject> get)
		{
			return OperateOnRelativeAtPath(from, relativePath, (operatingName, previous, current) =>
			{
				if (current == null && throwIfNotFound)
					throw new InvalidOperationException(relative + " wasn't found at path `" + relativePath + "`");
				return current;
			}, get);
		}

		private static GameObject OperateOnRelativeAtPath(
			GameObject inside, string relativePath,
			Func<string, GameObject, GameObject, GameObject> operate,
			Func<string, GameObject, GameObject> get)
		{
			if (inside == null)
				throw new ArgumentNullException("inside");

			string[] relatives = relativePath.Split('/');
			GameObject current = inside;
			int i = 0;
			do
			{
				var relative = relatives[i];
				var previous = current;
				current = get(relative, current);
				current = operate(relative, previous, current);
			} while (current != null && ++i < relatives.Length);

			return current == inside ? null : current;
		}

		public static void ToggleActive(this GameObject go)
		{
			go.SetActive(!go.activeSelf);
		}

		public static void SetActiveIfNot(this GameObject go, bool to)
		{
			if (go.activeSelf != to)
				go.SetActive(to);
	    }

	    public static GameObject Instantiate(this GameObject gameObject)
	    {
	        return UnityObject.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
	    }

        public static void Submit(this GameObject gameObject)
	    {
	        gameObject.Exectute(ExecuteEvents.submitHandler);
	    }

	    public static void Click(this GameObject gameObject)
	    {
	        gameObject.Exectute(ExecuteEvents.pointerClickHandler);
	    }
	    
	    public static void Down(this GameObject gameObject)
	    {
		    gameObject.Exectute(ExecuteEvents.pointerDownHandler);
	    }
	    
	    public static void Up(this GameObject gameObject)
	    {
		    gameObject.Exectute(ExecuteEvents.pointerUpHandler);
	    }

	    public static void Click(this EventTrigger eventTrigger, UnityAction<BaseEventData> callback)
	    {
	        eventTrigger.Trigger(EventTriggerType.PointerClick, callback);
	    }

	    public static void Down(this EventTrigger eventTrigger, UnityAction<BaseEventData> callback)
	    {
	        eventTrigger.Trigger(EventTriggerType.PointerDown, callback);
	    }

	    public static void Up(this EventTrigger eventTrigger, UnityAction<BaseEventData> callback)
	    {
	        eventTrigger.Trigger(EventTriggerType.PointerUp, callback);
	    }

	    public static void Trigger(this EventTrigger eventTrigger, EventTriggerType type,
	        UnityAction<BaseEventData> callback)
	    {
	        EventTrigger.Entry entry =
	            new EventTrigger.Entry { eventID = type };
	        entry.callback.AddListener(callback);
	        eventTrigger.triggers.Add(entry);
	    }

	    public static void Exectute<T>(this GameObject gameObject, ExecuteEvents.EventFunction<T> pointer) where T : IEventSystemHandler
	    {
	        ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), pointer);
        }
	    
        public static void ResetTransform(this Transform child)
	    {
	        child.localScale = Vector3.one;
	        child.localPosition = Vector3.zero;
	        child.localRotation = Quaternion.identity;
        }

	    public static IEnumerable<Transform> GetParents(this Transform transform, bool includeItself = false)
	    {
	        if (includeItself)
	        {
	            yield return transform;
	        }

	        while (transform.parent != null)
	        {
	            yield return transform.parent;
	            transform = transform.parent;
	        }
	    }
		
	    public static void Repeat(this MonoBehaviour behaviour, float interval, Action codeBlock)
	    {
		    behaviour.StartCoroutine(Repeat(interval, codeBlock));
	    }
	    
	    private static IEnumerator Repeat(float interval, Action codeBlock)
	    {
		    while (true)
		    {
			    codeBlock();
			    yield return new WaitForSecondsRealtime(interval);
		    }
	    }
		
		public static void RunDelayed(this MonoBehaviour behaviour, float delay, Action codeBlock)
		{
			if (behaviour && behaviour.isActiveAndEnabled)
				behaviour.StartCoroutine(RunDelayed(delay, codeBlock));
		}

		private static IEnumerator RunDelayed(float delay, Action codeBlock)
		{
			yield return new WaitForSecondsRealtime(delay);
			codeBlock();
		}
		
		public static Task WaitForEndOfFrame(this MonoBehaviour behaviour)
		{
			var tcs = new TaskCompletionSource<bool>();
			behaviour.StartCoroutine(WaitForEndOfFrame(() => tcs.SetResult(true)));
			return tcs.Task;
		}

		private static IEnumerator WaitForEndOfFrame(Action codeBlock)
		{
			yield return new WaitForEndOfFrame();
			codeBlock();
		}
		
		public static Mesh Combine(this Mesh mesh, bool mergeSubMeshes, params Mesh[] meshes)
		{
			CombineInstance[] combine = new CombineInstance[meshes.Length];
			for (int i = 0; i < meshes.Length; i++)
			{
				combine[i].mesh = meshes[i];
				combine[i].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
				combine[i].mesh.vertices = combine[i].mesh.vertices.Select(v => new Vector3(
					float.IsNaN(v.x) ? 0 : v.x,
					float.IsNaN(v.y) ? 0 : v.y,
					float.IsNaN(v.z) ? 0 : v.z
				)).ToArray();
			}

			mesh.CombineMeshes(combine, mergeSubMeshes);
			return mesh;
		}
		
		public static void SetLayerRecursive(this GameObject gameObject, int layer)
		{
			gameObject.layer = layer;
			for (var i = 0; i < gameObject.transform.childCount; i++)
			{
				var childGameObject = gameObject.transform.GetChild(i).gameObject;
				SetLayerRecursive(childGameObject, layer);
			}
		}
    }
}