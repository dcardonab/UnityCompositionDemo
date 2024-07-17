/*
 * Project Name: Unity Composition Demo
 * Script Name: Constants.cs
 * Description: This script defines constants used for this implementation, including scales and note generation modes.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using System.Collections.Generic;

public static class Constants
{
    public enum Scale
    {
        Ionian, Dorian, Phrygian, Lydian, Mixolydian, Aeolian, Locrian,
        WholeTone,
        FirstPentatonic, SecondPentatonic, ThirdPentatonic, FourthPentatonic, FifthPentatonic
    }
    
    public static Dictionary<Scale, int[]> Scales = new() {
        {Scale.Ionian, new []{0, 2, 4, 5, 7, 9, 11}},
        {Scale.Dorian, new []{0, 2, 3, 5, 7, 9, 10}},
        {Scale.Phrygian, new []{0, 1, 3, 5, 7, 8, 10}},
        {Scale.Lydian, new []{0, 2, 4, 6, 7, 9, 11}},
        {Scale.Mixolydian, new []{0, 2, 4, 5, 7, 9, 10}},
        {Scale.Aeolian, new []{0, 2, 3, 5, 7, 8, 10}},
        {Scale.Locrian, new []{0, 1, 3, 5, 6, 8, 10}},
        {Scale.WholeTone, new []{0, 2, 4, 6, 8, 10}},
        {Scale.FirstPentatonic, new []{0, 3, 5, 7, 10}},
        {Scale.SecondPentatonic, new []{0, 2, 4, 7, 9}},
        {Scale.ThirdPentatonic, new []{0, 2, 5, 7, 10}},
        {Scale.FourthPentatonic, new []{0, 3, 5, 8, 10}},
        {Scale.FifthPentatonic, new []{0, 2, 5, 7, 9}}
    };

    public enum NoteGenerationMode
    {
        Random,     // Random notes assigned when generating grid
        Markov      // First order Markov chain
    }
}
