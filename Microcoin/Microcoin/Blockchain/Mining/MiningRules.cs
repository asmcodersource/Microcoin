namespace Mining
{
    public class MiningRules
    {
        public IComplexityRule ComplexityRule { get; set; }
        public IRewardRule RewardRule { get; set; }

        public MiningRules(IComplexityRule complexityRule, IRewardRule rewardRule)
        {
            ComplexityRule = complexityRule;
            RewardRule = rewardRule;
        }
    }
}
