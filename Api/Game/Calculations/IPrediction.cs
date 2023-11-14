using System.Numerics;
using Api.Game.Objects;

namespace Api.Game.Calculations;

public interface IPrediction
{
    PredictionResult PredictPosition(IHero target, Vector3 sourcePosition, float delay, float speed, float radius, float range);
}