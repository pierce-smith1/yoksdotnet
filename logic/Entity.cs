using yoksdotnet.logic.scene;

namespace yoksdotnet.logic;

public class Entity
{
    public required PhysicalBasis basis;

    public Brand? brand;
    public Skin? skin;
    public Physics? physics;
    public PhysicsMeasurements? physicsMeasurements;

    public Emotion? emotion;
    public Trail? trail;
}
