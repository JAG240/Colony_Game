using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private int CalcH(Spot adjSpot, Spot end)
    {
        int dX = Mathf.Abs(adjSpot.x - end.x);
        int dY = Mathf.Abs(adjSpot.y - end.y);
        return dX + dY;
    }

    public List<Spot> GetPath(Spot start, Spot end)
    {
        List<Spot> openList = new List<Spot>();
        List<Spot> closeList = new List<Spot>();


        start.g = 0;
        openList.Add(start);

        while (openList.Count > 0)
        {
            int winner = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < openList[winner].f)
                {
                    winner = i;
                }
            }

            Spot currentSpot = openList[winner];

            if (currentSpot == end)
            {
                closeList.Add(end);
                break;
            }

            openList.Remove(currentSpot);

            foreach (Spot spot in currentSpot.adjSpots)
            {
                if (spot == null)
                {
                    continue;
                }

                spot.g = currentSpot.g + 1;
                spot.h = CalcH(spot, end);
                int tempF = spot.g + spot.h;
                Predicate<Spot> spotFinder = (Spot s) => { return s == spot; };

                if (openList.Contains(spot) && openList.Find(spotFinder).f < tempF)
                {
                    continue;
                }

                if (closeList.Contains(spot))
                {
                    continue;
                }

                spot.f = tempF;
                spot.parent = currentSpot;

                if (!openList.Contains(spot))
                {
                    openList.Add(spot);
                }
            }

            closeList.Add(currentSpot);
        }

        if(!closeList.Contains(end))
        {
            return null;
        }

        List<Spot> path = new List<Spot>();
        Spot tempSpot = end;
        path.Add(tempSpot);

        while (tempSpot != start)
        {
            tempSpot = tempSpot.parent;
            path.Add(tempSpot);
        }

        path.Reverse();

        return path;
    }

    public List<Spot> GetClosestTask(Spot currentPos)
    {
        List<Spot> openList = new List<Spot>();
        List<Spot> closeList = new List<Spot>();

        currentPos.g = 0;
        openList.Add(currentPos);
        Spot currentSpot = currentPos;

        while(openList.Count > 0)
        {
            int winner = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < openList[winner].f)
                {
                    winner = i;
                }
            }

            currentSpot = openList[winner];

            if (currentSpot.Task != null && currentSpot.Task.assignee == null && currentSpot.Task.obj.GetComponent<ObjectTaskScript>().GetInQueue()
                && currentPos.characterTasks.EnabledTasks.Contains(currentSpot.Task.taskType))
            {
                closeList.Add(currentSpot);
                break;
            }

            foreach (Spot spot in currentSpot.adjSpots)
            {
                if(spot == null)
                {
                    continue;
                }

                spot.g = currentSpot.g + 1;
                int tempF = spot.g;
                Predicate<Spot> spotFinder = (Spot s) => { return s == spot; };

                if (openList.Contains(spot) && openList.Find(spotFinder).f < tempF)
                {
                    continue;
                }

                if (closeList.Contains(spot))
                {
                    continue;
                }

                spot.f = tempF;
                spot.parent = currentSpot;

                if (!openList.Contains(spot))
                {
                    openList.Add(spot);
                }
            }

            openList.Remove(currentSpot);
            closeList.Add(currentSpot);
        }

        if(closeList[closeList.Count - 1].Task == null)
        {
            return null;
        }

        List<Spot> path = new List<Spot>();
        Spot tempSpot = currentSpot;
        path.Add(tempSpot);

        while (tempSpot != currentPos)
        {
            tempSpot = tempSpot.parent;
            path.Add(tempSpot);
        }

        path.Reverse();

        return path;
    }

    public List<Spot> GetClosestCharacter(Spot currentPos)
    {
        List<Spot> openList = new List<Spot>();
        List<Spot> closeList = new List<Spot>();
        bool successfulExit = false;

        currentPos.g = 0;
        openList.Add(currentPos);
        Spot currentSpot = currentPos;

        while (openList.Count > 0)
        {
            int winner = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < openList[winner].f)
                {
                    winner = i;
                }
            }

            currentSpot = openList[winner];

            if (currentSpot.characterTasks != null && !currentSpot.characterTasks.working && !currentSpot.characterTasks.charMove.GetMoving() && currentSpot.characterTasks.tasks.Count == 0
                && currentSpot.characterTasks.EnabledTasks.Contains(currentPos.Task.taskType))
            {
                successfulExit = true;
                closeList.Add(currentSpot);
                break;
            }

            foreach (Spot spot in currentSpot.adjSpots)
            {
                if (spot == null)
                {
                    continue;
                }

                spot.g = currentSpot.g + 1;
                int tempF = spot.g;
                Predicate<Spot> spotFinder = (Spot s) => { return s == spot; };

                if (openList.Contains(spot) && openList.Find(spotFinder).f < tempF)
                {
                    continue;
                }

                if (closeList.Contains(spot))
                {
                    continue;
                }

                spot.f = tempF;
                spot.parent = currentSpot;

                if (!openList.Contains(spot))
                {
                    openList.Add(spot);
                }
            }

            openList.Remove(currentSpot);
            closeList.Add(currentSpot);
        }

        if (!successfulExit)
        {
            return null;
        }

        List<Spot> path = new List<Spot>();
        Spot tempSpot = currentSpot;
        path.Add(tempSpot);

        while (tempSpot != currentPos)
        {
            tempSpot = tempSpot.parent;
            path.Add(tempSpot);
        }

        path.Reverse();

        return path;
    }
}