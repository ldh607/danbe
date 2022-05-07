using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig;
using CellBig.Common;


namespace CellBig
{
	public class ResourceLoader : MonoSingleton<ResourceLoader>
	{
		public class LoadedResource
		{
			public UnityEngine.Object m_Resource;
			public int m_ReferencedCount;

			public LoadedResource(UnityEngine.Object resource)
			{
				m_Resource = resource;
				m_ReferencedCount = 1;
			}
		}

		Dictionary<string, LoadedResource> loadedResources = new Dictionary<string, LoadedResource>();
		Dictionary<string, ResourceRequest> inProgressOperations = new Dictionary<string, ResourceRequest>();

		public IEnumerator Load<T>(string resourceName, System.Action<UnityEngine.Object> onComplete =null)
		{
			//Debug.LogFormat("Start to load {0} at frame {1}", resourceName, Time.frameCount);

			while (inProgressOperations.ContainsKey(resourceName))
				yield return null;

			if (loadedResources.ContainsKey(resourceName))
			{
				var resource = loadedResources[resourceName];
				if (resource != null && resource.m_Resource != null)
				{
					resource.m_ReferencedCount++;

					//Debug.LogFormat("{0} is loaded successfully at frame {1} : [RefCount] {2}", resourceName, Time.frameCount, resource.m_ReferencedCount);

					if (onComplete != null)
						onComplete(resource.m_Resource);
				}
				else
				{
					Debug.LogError("Resource already loaded but actual data is null. Name: " + resourceName);
				}
			}
			else
			{
				ResourceRequest request = Resources.LoadAsync(resourceName, typeof(T));
				inProgressOperations.Add(resourceName, request);

				yield return request;

				inProgressOperations.Remove(resourceName);

				if (request.asset != null)
				{
					var resource = new LoadedResource(request.asset);
					loadedResources.Add(resourceName, resource);

					//Debug.LogFormat("{0} is loaded successfully at frame {1} : [RefCount] {2}", resourceName, Time.frameCount, resource.m_ReferencedCount);

					if (onComplete != null)
						onComplete(request.asset);
				}
				else
				{
					Debug.LogError("Asynchronously loaded resource is null. Name: " + resourceName);
				}
			}
		}

		public float GetProgress(string resourceName)
		{
			if (loadedResources.ContainsKey(resourceName))
				return 1.0f;

			ResourceRequest request;
			inProgressOperations.TryGetValue(resourceName, out request);
			if (request != null)
			{
				return request.progress;
			}

			return 0.0f;
		}

		public void Unload(string resourceName)
		{
            if (loadedResources.ContainsKey(resourceName))
            {
                var resource = loadedResources[resourceName];
                if (resource != null && resource.m_Resource != null)
                {
                    if(resource.m_ReferencedCount > 1)
                        --resource.m_ReferencedCount;
                    else
                        loadedResources.Remove(resourceName);
                }
            }
		}

		public void UnloadAll()
		{
			loadedResources.Clear();
			Resources.UnloadUnusedAssets();
		}

#if UNITY_EDITOR
		void LogNotUnloadingResources()
		{
			foreach (var resPair in loadedResources)
			{
				var res = resPair.Value;
				if (res != null && res.m_Resource != null)
				{
                    Debug.LogWarningFormat("Not unloading resource : \"{0}\", RefCount : {1}", resPair.Key, res.m_ReferencedCount);
				}
			}
		}

		protected override void Release()
		{
			LogNotUnloadingResources();
		}
#endif
	}
}
