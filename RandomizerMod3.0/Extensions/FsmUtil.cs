using System;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomizerMod.Extensions
{
    public class MethodAction : FsmStateAction
    {
        public Action method;

        public override void Reset()
        {
            method = null;

            base.Reset();
        }

        public override void OnEnter()
        {
            if (method != null) method.Invoke();
            Finish();
        }
    }
    
    public static class FsmUtil
    {
        #region Get

        public static FsmState GetState(this PlayMakerFSM fsm, string stateName) => fsm.GetFsmState(stateName);
        public static FsmState GetFsmState(this PlayMakerFSM fsm, string stateName)
        {
            var fsmStates = fsm.FsmStates;
            int fsmStatesCount = fsmStates.Length;
            int i;
            for (i = 0; i < fsmStatesCount; i++)
            {
                if (fsmStates[i].Name == stateName)
                {
                    return fsmStates[i];
                }
            }
            return null;
        }

        public static FsmTransition GetTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetFsmTransition(stateName, eventName);
        public static FsmTransition GetFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetFsmState(stateName).GetFsmTransition(eventName);
        public static FsmTransition GetTransition(this FsmState state, string eventName) => state.GetFsmTransition(eventName);
        public static FsmTransition GetFsmTransition(this FsmState state, string eventName)
        {
            var transitions = state.Transitions;
            var transitionsCount = transitions.Length;
            int i;
            for (i = 0; i < transitionsCount; i++)
            {
                if (transitions[i].EventName == eventName)
                {
                    return transitions[i];
                }
            }
            return null;
        }

        public static TAction GetAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction => fsm.GetFsmAction<TAction>(stateName, index);
        public static TAction GetFsmAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction
        {
            return (TAction) fsm.GetFsmState(stateName).Actions[index];
        }
        
        public static void RemoveActionsOfType<T>(this FsmState self) where T : FsmStateAction
        {
            self.Actions = self.Actions.Where(action => !(action is T)).ToArray();
        }

        public static T GetActionOfType<T>(this FsmState self) where T : FsmStateAction
        {
            return self.Actions.OfType<T>().FirstOrDefault();
        }

        public static T[] GetActionsOfType<T>(this FsmState self) where T : FsmStateAction
        {
            return self.Actions.OfType<T>().ToArray();
        }

        public static void ClearTransitions(this FsmState self)
        {
            self.Transitions = new FsmTransition[0];
        }
        
        public static void AddFirstAction(this FsmState self, FsmStateAction action)
        {
            FsmStateAction[] actions = new FsmStateAction[self.Actions.Length + 1];
            Array.Copy(self.Actions, 0, actions, 1, self.Actions.Length);
            actions[0] = action;

            self.Actions = actions;
        }
        
        public static GameObject FindGameObjectInChildren(this GameObject gameObject, string name)
        {
            if (gameObject == null)
            {
                return null;
            }

            return gameObject.GetComponentsInChildren<Transform>(true)
                .Where(t => t.name == name)
                .Select(t => t.gameObject).FirstOrDefault();
        }
        
        public static void RemoveTransitionsTo(this FsmState self, string toState)
        {
            self.Transitions = self.Transitions.Where(transition => transition.ToState != toState).ToArray();
        }
        
        public static GameObject FindGameObject(this Scene scene, string name)
        {
            if (!scene.IsValid())
            {
                return null;
            }

            try
            {
                foreach (GameObject go in scene.GetRootGameObjects())
                {
                    if (go == null)
                    {
                        continue;
                    }

                    GameObject found = go.FindGameObjectInChildren(name);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Log("FindGameObject failed:\n" + e.Message);
            }

            return null;
        }
        
        public static PlayMakerFSM LocateFSM(this GameObject self, string fsmName)
        {
            return FSMUtility.LocateFSM(self, fsmName);
        }
        
        public static PlayMakerFSM LocateFSM(this Component self, string fsmName)
        {
            return self.gameObject.LocateFSM(fsmName);
        }

        #endregion

        #region Add

        public static FsmState AddState(this PlayMakerFSM fsm, string stateName) => fsm.AddFsmState(stateName);
        public static FsmState AddFsmState(this PlayMakerFSM fsm, string stateName) => fsm.AddFsmState(new FsmState(fsm.Fsm) { Name = stateName });
        public static FsmState AddState(this PlayMakerFSM fsm, FsmState state) => fsm.AddFsmState(state);
        public static FsmState AddFsmState(this PlayMakerFSM fsm, FsmState state)
        {
            FsmState[] origStates = fsm.FsmStates;
            FsmState[] states = new FsmState[origStates.Length + 1];
            origStates.CopyTo(states, 0);
            states[origStates.Length] = state;
            fsm.Fsm.States = states;
            return states[origStates.Length];
        }

        public static FsmState CopyState(this PlayMakerFSM fsm, string fromState, string toState) => fsm.CopyFsmState(fromState, toState);
        public static FsmState CopyFsmState(this PlayMakerFSM fsm, string fromState, string toState)
        {
            FsmState copy = new FsmState(fsm.GetFsmState(fromState))
            {
                Name = toState
            };
            FsmTransition[] transitions = copy.Transitions;
            int transitionsCount = transitions.Length;
            int i;
            for (i = 0; i < transitionsCount; i++)
            {
                // because playmaker is bad, so this has to be done extra
                transitions[i].ToFsmState = fsm.GetFsmState(transitions[i].ToState);
            }
            fsm.AddFsmState(copy);
            return copy;
        }

        public static FsmEvent AddTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.AddFsmTransition(stateName, eventName, toState);
        public static FsmEvent AddFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetFsmState(stateName).AddFsmTransition(eventName, toState);
        public static FsmEvent AddTransition(this FsmState state, string eventName, string toState) => state.AddFsmTransition(eventName, toState);
        public static FsmEvent AddFsmTransition(this FsmState state, string eventName, string toState)
        {
            var ret = FsmEvent.GetFsmEvent(eventName);
            FsmTransition[] origTransitions = state.Transitions;
            FsmTransition[] transitions = new FsmTransition[origTransitions.Length + 1];
            origTransitions.CopyTo(transitions, 0);
            transitions[origTransitions.Length] = new FsmTransition
            {
                ToState = toState,
                ToFsmState = state.Fsm.GetState(toState),
                FsmEvent = ret
            };
            state.Transitions = transitions;
            return ret;
        }

        public static FsmEvent AddGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState) => fsm.AddFsmGlobalTransitions(globalEventName, toState);
        public static FsmEvent AddFsmGlobalTransitions(this PlayMakerFSM fsm, string globalEventName, string toState)
        {
            var ret = new FsmEvent(globalEventName) { IsGlobal = true };
            FsmTransition[] origTransitions = fsm.FsmGlobalTransitions;
            FsmTransition[] transitions = new FsmTransition[origTransitions.Length + 1];
            origTransitions.CopyTo(transitions, 0);
            transitions[origTransitions.Length] = new FsmTransition
            {
                ToState = toState,
                ToFsmState = fsm.GetState(toState),
                FsmEvent = ret
            };
            fsm.Fsm.GlobalTransitions = transitions;
            return ret;
        }

        public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => fsm.AddFsmAction(stateName, action);
        public static void AddFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => fsm.GetFsmState(stateName).AddFsmAction(action);
        public static void AddAction(this FsmState state, FsmStateAction action) => state.AddFsmAction(action);
        public static void AddFsmAction(this FsmState state, FsmStateAction action)
        {
            FsmStateAction[] origActions = state.Actions;
            FsmStateAction[] actions = new FsmStateAction[origActions.Length + 1];
            origActions.CopyTo(actions, 0);
            actions[origActions.Length] = action;
            state.Actions = actions;
        }

        public static void AddMethod(this PlayMakerFSM fsm, string stateName, Action method) => fsm.GetFsmState(stateName).AddMethod(method);
        public static void AddMethod(this FsmState state, Action method)
        {
            state.AddFsmAction(new MethodAction() { method = method });
        }

        #endregion

        #region Insert

        public static void InsertAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.InsertFsmAction(stateName, action, index);
        public static void InsertFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.GetFsmState(stateName).InsertFsmAction(action, index);
        public static void InsertAction(this FsmState state, FsmStateAction action, int index) => state.InsertFsmAction(action, index);
        public static void InsertFsmAction(this FsmState state, FsmStateAction action, int index)
        {
            FsmStateAction[] origActions = state.Actions;
            FsmStateAction[] actions = new FsmStateAction[origActions.Length + 1];
            int i;
            for (i = 0; i < index; i++)
            {
                actions[i] = origActions[i];
            }
            actions[index] = action;
            for (i = index; i < origActions.Length; i++)
            {
                actions[i + 1] = origActions[i];
            }

            state.Actions = actions;
        }

        public static void InsertMethod(this PlayMakerFSM fsm, string stateName, Action method, int index) => fsm.GetFsmState(stateName).InsertMethod(method, index);
        public static void InsertMethod(this FsmState state, Action method, int index)
        {
            state.InsertFsmAction(new MethodAction() { method = method }, index);
        }

        #endregion

        #region Change

        public static void ChangeTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.ChangeFsmTransition(stateName, eventName, toState);
        public static void ChangeFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetFsmState(stateName).ChangeFsmTransition(eventName, toState);
        public static void ChangeTransition(this FsmState state, string eventName, string toState) => state.ChangeFsmTransition(eventName, toState);
        public static void ChangeFsmTransition(this FsmState state, string eventName, string toState)
        {
            var transition = state.GetFsmTransition(eventName);
            transition.ToState = toState;
            transition.ToFsmState = state.Fsm.GetState(toState);
        }

        #endregion

        #region Remove

        public static void RemoveState(this PlayMakerFSM fsm, string stateName) => fsm.RemoveFsmState(stateName);
        public static void RemoveFsmState(this PlayMakerFSM fsm, string stateName)
        {
            FsmState[] origStates = fsm.FsmStates;
            FsmState[] newStates = new FsmState[origStates.Length - 1];
            int i;
            int foundInt = 0;
            for (i = 0; i < newStates.Length; i++)
            {
                if (origStates[i].Name == stateName)
                {
                    foundInt = 1;
                }
                newStates[i] = origStates[i + foundInt];
            }

            fsm.Fsm.States = newStates;
        }

        public static void RemoveTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.RemoveFsmTransition(stateName, eventName);
        public static void RemoveFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetState(stateName).RemoveFsmTransition(eventName);
        public static void RemoveTransition(this FsmState state, string eventName) => state.RemoveFsmTransition(eventName);
        public static void RemoveFsmTransition(this FsmState state, string eventName)
        {
            FsmTransition[] origTransitions = state.Transitions;
            FsmTransition[] newTransitions = new FsmTransition[origTransitions.Length - 1];
            int i;
            int foundInt = 0;
            for (i = 0; i < newTransitions.Length; i++)
            {
                if (origTransitions[i].EventName == eventName)
                {
                    foundInt = 1;
                }
                newTransitions[i] = origTransitions[i + foundInt];
            }

            state.Transitions = newTransitions;
        }

        public static void RemoveAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.RemoveFsmAction(stateName, index);
        public static void RemoveFsmAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetFsmState(stateName).RemoveFsmAction(index);
        public static void RemoveAction(this FsmState state, int index) => state.RemoveFsmAction(index);
        public static void RemoveFsmAction(this FsmState state, int index)
        {
            FsmStateAction[] origActions = state.Actions;
            FsmStateAction[] actions = new FsmStateAction[origActions.Length - 1];
            int i;
            for (i = 0; i < index; i++)
            {
                actions[i] = origActions[i];
            }
            for (i = index; i < actions.Length; i++)
            {
                actions[i] = origActions[i + 1];
            }

            state.Actions = actions;
        }

        #endregion

        #region FSM Variables

        private static TVar[] makeNewVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
        {
            TVar[] newArray = new TVar[orig.Length + 1];
            orig.CopyTo(newArray, 0);
            newArray[orig.Length] = new TVar() { Name = name };
            return newArray;
        }
        public static FsmFloat AddFloatVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmFloatVariable(name);
        public static FsmFloat AddFsmFloatVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmFloat>(fsm.FsmVariables.FloatVariables, name);
            fsm.FsmVariables.FloatVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        public static FsmInt AddIntVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmIntVariable(name);
        public static FsmInt AddFsmIntVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmInt>(fsm.FsmVariables.IntVariables, name);
            fsm.FsmVariables.IntVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        public static FsmBool AddBoolVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmBoolVariable(name);
        public static FsmBool AddFsmBoolVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmBool>(fsm.FsmVariables.BoolVariables, name);
            fsm.FsmVariables.BoolVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        public static FsmString AddStringVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmStringVariable(name);
        public static FsmString AddFsmStringVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmString>(fsm.FsmVariables.StringVariables, name);
            fsm.FsmVariables.StringVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        public static FsmVector2 AddVector2Variable(this PlayMakerFSM fsm, string name) => fsm.AddFsmVector2Variable(name);
        public static FsmVector2 AddFsmVector2Variable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmVector2>(fsm.FsmVariables.Vector2Variables, name);
            fsm.FsmVariables.Vector2Variables = tmp;
            return tmp[tmp.Length - 1];
        }
        public static FsmVector3 AddVector3Variable(this PlayMakerFSM fsm, string name) => fsm.AddFsmVector3Variable(name);
        public static FsmVector3 AddFsmVector3Variable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmVector3>(fsm.FsmVariables.Vector3Variables, name);
            fsm.FsmVariables.Vector3Variables = tmp;
            return tmp[tmp.Length - 1];
        }
        public static FsmColor AddColorVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmColorVariable(name);
        public static FsmColor AddFsmColorVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmColor>(fsm.FsmVariables.ColorVariables, name);
            fsm.FsmVariables.ColorVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        public static FsmRect AddRectVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmRectVariable(name);
        public static FsmRect AddFsmRectVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmRect>(fsm.FsmVariables.RectVariables, name);
            fsm.FsmVariables.RectVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        public static FsmQuaternion AddQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmQuaternionVariable(name);
        public static FsmQuaternion AddFsmQuaternionVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmQuaternion>(fsm.FsmVariables.QuaternionVariables, name);
            fsm.FsmVariables.QuaternionVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        public static FsmGameObject AddGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmGameObjectVariable(name);
        public static FsmGameObject AddFsmGameObjectVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmGameObject>(fsm.FsmVariables.GameObjectVariables, name);
            fsm.FsmVariables.GameObjectVariables = tmp;
            return tmp[tmp.Length - 1];
        }

        private static TVar findInVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
        {
            int count = orig.Length;
            int i;
            for (i = 0; i < count; i++)
            {
                if (orig[i].Name == name)
                {
                    return orig[i];
                }
            }
            return null;
        }
        public static FsmFloat FindFloatVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmFloatVariable(name);
        public static FsmFloat FindFsmFloatVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmFloat>(fsm.FsmVariables.FloatVariables, name);
        public static FsmInt FindIntVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmIntVariable(name);
        public static FsmInt FindFsmIntVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmInt>(fsm.FsmVariables.IntVariables, name);
        public static FsmBool FindBoolVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmBoolVariable(name);
        public static FsmBool FindFsmBoolVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmBool>(fsm.FsmVariables.BoolVariables, name);
        public static FsmString FindStringVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmStringVariable(name);
        public static FsmString FindFsmStringVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmString>(fsm.FsmVariables.StringVariables, name);
        public static FsmVector2 FindVector2Variable(this PlayMakerFSM fsm, string name) => fsm.FindFsmVector2Variable(name);
        public static FsmVector2 FindFsmVector2Variable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmVector2>(fsm.FsmVariables.Vector2Variables, name);
        public static FsmVector3 FindVector3Variable(this PlayMakerFSM fsm, string name) => fsm.FindFsmVector3Variable(name);
        public static FsmVector3 FindFsmVector3Variable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmVector3>(fsm.FsmVariables.Vector3Variables, name);
        public static FsmColor FindColorVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmColorVariable(name);
        public static FsmColor FindFsmColorVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmColor>(fsm.FsmVariables.ColorVariables, name);
        public static FsmRect FindRectVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmRectVariable(name);
        public static FsmRect FindFsmRectVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmRect>(fsm.FsmVariables.RectVariables, name);
        public static FsmQuaternion FindQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmQuaternionVariable(name);
        public static FsmQuaternion FindFsmQuaternionVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmQuaternion>(fsm.FsmVariables.QuaternionVariables, name);
        public static FsmGameObject FindGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmGameObjectVariable(name);
        public static FsmGameObject FindFsmGameObjectVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmGameObject>(fsm.FsmVariables.GameObjectVariables, name);

        public static FsmFloat GetFloatVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmFloatVariable(name);
        public static FsmFloat GetFsmFloatVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindFloatVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddFloatVariable(name);
        }
        public static FsmInt GetIntVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmIntVariable(name);
        public static FsmInt GetFsmIntVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindIntVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddIntVariable(name);
        }
        public static FsmBool GetBoolVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmBoolVariable(name);
        public static FsmBool GetFsmBoolVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindBoolVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddBoolVariable(name);
        }
        public static FsmString GetStringVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmStringVariable(name);
        public static FsmString GetFsmStringVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindStringVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddStringVariable(name);
        }
        public static FsmVector2 GetVector2Variable(this PlayMakerFSM fsm, string name) => fsm.GetFsmVector2Variable(name);
        public static FsmVector2 GetFsmVector2Variable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindVector2Variable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddVector2Variable(name);
        }
        public static FsmVector3 GetVector3Variable(this PlayMakerFSM fsm, string name) => fsm.GetFsmVector3Variable(name);
        public static FsmVector3 GetFsmVector3Variable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindVector3Variable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddVector3Variable(name);
        }
        public static FsmColor GetColorVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmColorVariable(name);
        public static FsmColor GetFsmColorVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindColorVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddColorVariable(name);
        }
        public static FsmRect GetRectVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmRectVariable(name);
        public static FsmRect GetFsmRectVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindRectVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddRectVariable(name);
        }
        public static FsmQuaternion GetQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmQuaternionVariable(name);
        public static FsmQuaternion GetFsmQuaternionVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindQuaternionVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddQuaternionVariable(name);
        }
        public static FsmGameObject GetGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmGameObjectVariable(name);
        public static FsmGameObject GetFsmGameObjectVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindGameObjectVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddGameObjectVariable(name);
        }

        #endregion
    }
}
