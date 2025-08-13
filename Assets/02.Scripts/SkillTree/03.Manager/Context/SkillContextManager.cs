// using System.Collections.Generic;
// using Fusion;
//
// public class SkillContextManager : BehaviourSingleton<SkillContextManager>
// {
//     private readonly Dictionary<NetworkId, SkillContext> _contexts = new();
//     
//     public SkillContext Get(NetworkId id) => _contexts[id];
//
//     public void Add(NetworkId id, SkillContext skillContext)
//     {
//         _contexts[id] = skillContext;
//     }
//
//     public void Remove(NetworkId id)
//     {
//         _contexts.Remove(id);
//     }
// }