namespace EnTTSharp.Entities
{
    public partial class EntityRegistry<TEntityKey>
    {
        public void DiscardView<TView>() where TView : IEntityView<TEntityKey>
        {
            views.Remove(typeof(TView));
        }
    }
}