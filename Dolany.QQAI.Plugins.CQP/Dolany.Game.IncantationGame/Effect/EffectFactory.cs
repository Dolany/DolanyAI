namespace Dolany.Game.IncantationGame.Effect
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class EffectFactory
    {
        public static IEnumerable<IEffect> AllEffects { get; } = InitEffects();

        private static IEnumerable<IEffect> InitEffects()
        {
            var assembly = Assembly.GetAssembly(typeof(IEffect));
            var types = assembly.GetTypes();
            return types.Where(type => typeof(IEffect).IsAssignableFrom(type) && type.IsClass)
                .Select(type => assembly.CreateInstance(type.FullName) as IEffect);
        }
    }
}
