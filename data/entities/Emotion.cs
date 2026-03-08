using System;

namespace yoksdotnet.data.entities;

public class Emotion : EntityComponent
{
    public required double ambition;
    public required double empathy;
    public required double optimism;

    public double Magnitude => Math.Sqrt(ambition * ambition + empathy * empathy + optimism * optimism);
}
