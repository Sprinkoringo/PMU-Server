/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


using System;
using System.Collections.Generic;
using System.Text;
using Server.RDungeons;
using Server.Maps;

namespace Server.DataConverter.RDungeons.V2
{
	public class RDungeonFloor
    {
        GeneratorOptions options;
			
        public GeneratorOptions Options { get {return options;} }
			
        
        public int Darkness { get; set; }

        

        public Enums.RFloorGoalType GoalType { get; set; }

        public int GoalMap { get; set; }
        public int GoalX { get; set; }
        public int GoalY { get; set; }
        
        public string Music { get; set; }
        
        
        
        #region Terrain Variables

        public int StairsX { get; set; }
        public int StairsSheet { get; set; }

        public int mGroundX { get; set; }
        public int mGroundSheet { get; set; }

        public int mTopLeftX { get; set; }
        public int mTopLeftSheet { get; set; }
        public int mTopCenterX { get; set; }
        public int mTopCenterSheet { get; set; }
        public int mTopRightX { get; set; }
        public int mTopRightSheet { get; set; }

        public int mCenterLeftX { get; set; }
        public int mCenterLeftSheet { get; set; }
        public int mCenterCenterX { get; set; }
        public int mCenterCenterSheet { get; set; }
        public int mCenterRightX { get; set; }
        public int mCenterRightSheet { get; set; }

        public int mBottomLeftX { get; set; }
        public int mBottomLeftSheet { get; set; }
        public int mBottomCenterX { get; set; }
        public int mBottomCenterSheet { get; set; }
        public int mBottomRightX { get; set; }
        public int mBottomRightSheet { get; set; }

        public int mInnerTopLeftX { get; set; }
        public int mInnerTopLeftSheet { get; set; }
        public int mInnerBottomLeftX { get; set; }
        public int mInnerBottomLeftSheet { get; set; }
        public int mInnerTopRightX { get; set; }
        public int mInnerTopRightSheet { get; set; }
        public int mInnerBottomRightX { get; set; }
        public int mInnerBottomRightSheet { get; set; }

        public int mIsolatedWallX { get; set; }
        public int mIsolatedWallSheet { get; set; }
        public int mColumnTopX { get; set; }
        public int mColumnTopSheet { get; set; }
        public int mColumnCenterX { get; set; }
        public int mColumnCenterSheet { get; set; }
        public int mColumnBottomX { get; set; }
        public int mColumnBottomSheet { get; set; }

        public int mRowLeftX { get; set; }
        public int mRowLeftSheet { get; set; }
        public int mRowCenterX { get; set; }
        public int mRowCenterSheet { get; set; }
        public int mRowRightX { get; set; }
        public int mRowRightSheet { get; set; }
        
        public int mWaterX { get; set; }
        public int mWaterSheet { get; set; }
        public int mWaterAnimX { get; set; }
        public int mWaterAnimSheet { get; set; }

        public int mShoreTopLeftX { get; set; }
        public int mShoreTopLeftSheet { get; set; }
        public int mShoreTopRightX { get; set; }
        public int mShoreTopRightSheet { get; set; }
        public int mShoreBottomRightX { get; set; }
        public int mShoreBottomRightSheet { get; set; }
        public int mShoreBottomLeftX { get; set; }
        public int mShoreBottomLeftSheet { get; set; }
        public int mShoreDiagonalForwardX { get; set; }
        public int mShoreDiagonalForwardSheet { get; set; }
        public int mShoreDiagonalBackX { get; set; }
        public int mShoreDiagonalBackSheet { get; set; }
        public int mShoreTopX { get; set; }
        public int mShoreTopSheet { get; set; }
        public int mShoreRightX { get; set; }
        public int mShoreRightSheet { get; set; }
        public int mShoreBottomX { get; set; }
        public int mShoreBottomSheet { get; set; }
        public int mShoreLeftX { get; set; }
        public int mShoreLeftSheet { get; set; }
        public int mShoreVerticalX { get; set; }
        public int mShoreVerticalSheet { get; set; }
        public int mShoreHorizontalX { get; set; }
        public int mShoreHorizontalSheet { get; set; }
        public int mShoreInnerTopLeftX { get; set; }
        public int mShoreInnerTopLeftSheet { get; set; }
        public int mShoreInnerTopRightX { get; set; }
        public int mShoreInnerTopRightSheet { get; set; }
        public int mShoreInnerBottomRightX { get; set; }
        public int mShoreInnerBottomRightSheet { get; set; }
        public int mShoreInnerBottomLeftX { get; set; }
        public int mShoreInnerBottomLeftSheet { get; set; }
        public int mShoreInnerTopX { get; set; }
        public int mShoreInnerTopSheet { get; set; }
        public int mShoreInnerRightX { get; set; }
        public int mShoreInnerRightSheet { get; set; }
        public int mShoreInnerBottomX { get; set; }
        public int mShoreInnerBottomSheet { get; set; }
        public int mShoreInnerLeftX { get; set; }
        public int mShoreInnerLeftSheet { get; set; }
        public int mShoreSurroundedX { get; set; }
        public int mShoreSurroundedSheet { get; set; }

        #endregion

        public int ItemSpawnRate { get; set; }
        public int[] Items { get; set; }
        
        public MapNpcPreset[] Npc { get; set; }
        public List<int> Traps { get; set; }
        public List <Enums.Weather> Weather { get; set; }

        public RDungeonFloor() {
            Traps = new List<int>();
            Weather = new List<Enums.Weather>();
            Npc = new MapNpcPreset[Constants.MAX_MAP_NPCS];
            for (int i = 0; i < Npc.Length; i++) {
                Npc[i] = new MapNpcPreset();
            }
            Items = new int[16];
            options = new GeneratorOptions();
        }
    }
}
