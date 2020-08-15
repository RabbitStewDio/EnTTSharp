using System;
using EnttSharp.Entities;
using EntityKey = EnTTSharp.Entities.EntityKey;

namespace EnTTSharp.Test
{
    public static class DocumentationExample
    {
        public readonly struct Position
        {
            public readonly double X;
            public readonly double Y;

            public Position(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        public readonly struct Velocity
        {
            public readonly double DeltaX;
            public readonly double DeltaY;

            public Velocity(double deltaX, double deltaY)
            {
                DeltaX = deltaX;
                DeltaY = deltaY;
            }
        }

        public static void UpdatePosition(Entities.EntityRegistry<EntityKey> registry, TimeSpan deltaTime)
        {
            // view contains all the entities that have both a position and a velocty component ...
            var view = registry.View<Velocity, Position>();
            foreach (var entity in view)
            {
                if (view.GetComponent(entity, out Position pos) &&
                    view.GetComponent(entity, out Velocity velocity))
                {
                    var posChanged = new Position(pos.X + velocity.DeltaX * deltaTime.TotalSeconds,
                                                  pos.Y + velocity.DeltaY * deltaTime.TotalSeconds);
                    registry.AssignComponent(entity, in posChanged);
                }
            }
        }

        public static void ClearVelocity(Entities.EntityRegistry<EntityKey> registry)
        {
            var view = registry.View<Velocity>();
            foreach (var entity in view)
            {
                registry.AssignComponent(entity, new Velocity(0,0));
            }
        }

        public static void DummyMain(string [] args)
        {
            var random = new Random();
            var registry = new Entities.EntityRegistry<EntityKey>(EntityKey.MaxAge, (age, id) => new EntityKey(age, id));
            registry.Register<Velocity>();
            registry.Register<Position>();

            for (int x = 0; x < 10; x += 1)
            {
                var entity = registry.Create();
                registry.AssignComponent<Position>(entity);
                if ((x % 2) == 0)
                {
                    registry.AssignComponent(entity, new Velocity(random.NextDouble(), random.NextDouble()));
                }
            }

            UpdatePosition(registry, TimeSpan.FromSeconds(0.24));
            ClearVelocity(registry);
        }
    }
}