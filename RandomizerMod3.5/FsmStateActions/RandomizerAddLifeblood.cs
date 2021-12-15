using HutongGames.PlayMaker;

namespace RandomizerMod.FsmStateActions
{
    internal class RandomizerAddLifeblood : FsmStateAction
    {
        private int _amount;

        public RandomizerAddLifeblood(int amount)
        {
            _amount = amount;
        }

        public override void OnEnter()
        {
            for (int i = 0; i < _amount; i++)
            {
                EventRegister.SendEvent("ADD BLUE HEALTH");
            }

            Finish();
        }
    }
}