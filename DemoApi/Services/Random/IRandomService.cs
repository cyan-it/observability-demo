﻿namespace DemoApi.Services.Random;

public interface IRandomService
{
    int Next(int min, int max);
    double NextDouble();
}