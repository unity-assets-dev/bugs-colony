# Bug Colony 

**web-deploy**: [link](https://unity-assets-dev.github.io/bugs-colony/)

Used plugins: **Zenject, DoTween\PrimeTween, NaugthlyAttributes**
Based on custom selfmade UIKit;
Patterns to use: **Strategy, DoubleDispatch, Command, AbstactFactory, Proxy, UI MVVM, StateMachine, States**

NavMesh Agent through the HeatMap which I realized with a BurstCompile and IJobParallelFor multithread

### Heat map example ###
![HeatMap example](https://unity-assets-dev.github.io/bugs-colony/heat-map.jpg)
___
## Description

Bug Colony is a 3D simulation of a bug colony. Bugs move around the scene, collect resources, reproduce and mutate.
## Camera

Top down view
## Graphics

No graphics required. Boxes, spheres, capsules and any other primitives are fine. You are free to use any assets from asset stores or any other resources, but note that graphic this will not affect final results at all.
## Gameplay

Resources (food) randomly appear on the scene over time.
## Worker Bug

Worker bug moves around the scene and picks up resources. When a worker has eaten 2 resources, it splits into 2 worker bugs.
If there are more than 10 alive bugs in the colony, each time a worker splits there is a 10% chance that one of the offspring mutates into a predator bug.
## Predator Bug

Predator bug attacks and eats everything: other bugs (both workers and other predators) and food resources. Predator bug lives for 10 seconds, then dies.
When a predator has eaten 3 resources or bugs, it splits into 2 predator bugs (each with a fresh 10 second timer).
## Colony Rules

If there are no alive bugs left on the scene, a new worker bug spawns automatically.
## UI

Display the following counters:
Total dead worker bugs
Total dead predator bugs
UI should be created using uGUI.

## What we are looking for:

The most important thing we evaluate is code architecture. We expect to see clean, easy to extend and easy to understand code. Imagine that in the near future we will add 10 new bug types with different behaviors (flying bugs, burrowing bugs, healer bugs, etc.)
We also pay attention to the tools and frameworks you use. If you are familiar with DI containers, UniTask, R3/UniRx or other professional Unity development tools, please use them.
If you have experience with architecture patterns (Strategy, Factory, Object Pool, etc.) please demonstrate them.

## Technical Requirements

Unity 6
C#
Do NOT use ECS/DOTS Yes. We use DOTS in our project but we are interested in OOP knowledge


