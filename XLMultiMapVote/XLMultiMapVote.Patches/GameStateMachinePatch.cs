using HarmonyLib;
using GameManagement;
using System.Collections.Generic;
using UnityEngine;
using XLMultiMapVote.Data;
using System;

namespace XLMultiMapVote.Patches
{
    [HarmonyPatch(typeof(GameStateMachine))]
    public static class GameStateMachinePatch
    {
        // Note: Specifying the method name and argument types ensures the correct method is patched.
        [HarmonyPatch(nameof(GameStateMachine.RequestTransitionTo), new Type[] { typeof(GameState), typeof(bool), typeof(Action<GameState>) })]
        [HarmonyPostfix]
        public static void Postfix(GameStateMachine __instance, GameState requestedState, bool alwaysAddToNavStack, ref Stack<GameState> ___stateNavigationStack)
        {
            // Check if the requested state is the custom game state and whether it should be added to the navigation stack
            if (requestedState is VoteState)
            {
                ___stateNavigationStack.Push(requestedState);
            }
        }
    }
}