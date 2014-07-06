namespace Script 
{
	using System;
    using System.Collections.Generic;
    using System.Text;

    using Server;
    using Server.Maps;
    using Server.Players;
    using Server.RDungeons;
    using Server.Dungeons;
    using Server.Combat;
    using Server.Pokedex;
    using Server.Items;
    using Server.Moves;
    using Server.Npcs;
    using Server.Stories;
    using Server.Exp;
    using Server.Network;
    using PMU.Sockets;
    using Server.Players.Parties;
    using Server.Logging;
    using Server.Missions;
    using Server.Events.Player.TriggerEvents;
    using Server.WonderMails;
    using Server.Tournaments;
    
	public class RandomIce
	{
		public const int VOID = -1;
		public const int ICE = 0;
		public const int START = 1;
		public const int END = 2;
		public const int BLOCK = 3;
		public const int GROUND = 4;
		public const int PATHVERT = 5;
		public const int PATHHORIZ = 6;
		public const int PATHCROSS = 7;
		public const int PATHDOWN = 8;
		public const int PATHLEFT = 9;
		public const int PATHUP = 10;
		public const int PATHRIGHT = 11;
		public const int TRUEPATHVERT = 12;
		public const int TRUEPATHHORIZ = 13;
		
		#region IceMap
		
		public static int[,] GenIcePuzzle(int maxX, int maxY, int bends, int branchChance, int branchDepth, int blockRatio, int security, bool stableStart, bool stableEnd) {
			int[,] mapArray = new int[maxX, maxY];
			int[,] seqArray = new int[maxX, maxY];
			List<int> nodeDirs = new List<int>();
			
			List<int> xPoints = new List<int>();
			List<int> yPoints = new List<int>();
			bool firstPathVert = false;
			
			GenIcePath(maxX, maxY, bends, xPoints, yPoints, ref firstPathVert, stableStart, stableEnd);
			
			texturePath(xPoints, yPoints, mapArray, seqArray, nodeDirs, firstPathVert);
			textureNodes(blockRatio, xPoints, yPoints, mapArray, maxX, maxY, firstPathVert);
			//secure branches
			for (int i = xPoints.Count - 2; i >= 0; i--) {
				checkBranches(i, blockRatio, security, maxX, maxY,
				xPoints, yPoints, mapArray, seqArray, nodeDirs);
			}
		
			//add extra branches
			for (int i = xPoints.Count - 2; i >= 0; i--) {
				addExtraBranches(i, branchChance, branchDepth, blockRatio, security, maxX, maxY,
				xPoints, yPoints, mapArray, seqArray, nodeDirs);
			}
			
			return mapArray;
		}
		
		static void texturePath(List<int> xPoints, List<int> yPoints, int[,] mapArray, int[,] seqArray, List<int> nodeDirs, bool firstPathVert) {
			for (int i = 0; i < xPoints.Count - 1; i++) {
				seqArray[xPoints[i],yPoints[i]] = i+1;
				if ((i % 2 == 0) == firstPathVert) {
					//path is vertical
					if (yPoints[i] < yPoints[i+1]) {
						//going down
						nodeDirs.Add(0);
						for (int j = yPoints[i] + 1; j <= yPoints[i+1]; j++) {
							if (mapArray[xPoints[i],j] == PATHHORIZ) {
								mapArray[xPoints[i],j] = PATHCROSS;
							} else {
								mapArray[xPoints[i],j] = PATHVERT;
							}
							seqArray[xPoints[i],j] = i+1;
						}
						mapArray[xPoints[i+1],yPoints[i+1]] = PATHDOWN;
					} else {
						//going up
						nodeDirs.Add(2);
						for (int j = yPoints[i] - 1; j >= yPoints[i+1]; j--) {
							if (mapArray[xPoints[i],j] == PATHHORIZ) {
								mapArray[xPoints[i],j] = PATHCROSS;
							} else {
								mapArray[xPoints[i],j] = PATHVERT;
							}
							seqArray[xPoints[i],j] = i+1;
						}
						mapArray[xPoints[i+1],yPoints[i+1]] = PATHUP;
					}
				} else {
					//path is horizontal
					if (xPoints[i] < xPoints[i+1]) {
						//going right
						nodeDirs.Add(3);
						for (int j = xPoints[i] + 1; j <= xPoints[i+1]; j++) {
							if (mapArray[j,yPoints[i]] == PATHVERT) {
								mapArray[j,yPoints[i]] = PATHCROSS;
							} else {
								mapArray[j,yPoints[i]] = PATHHORIZ;
							}
							seqArray[j,yPoints[i]] = i+1;
						}
						mapArray[xPoints[i+1],yPoints[i+1]] = PATHRIGHT;
					} else {
						//going left
						nodeDirs.Add(1);
						for (int j = xPoints[i] - 1; j >= xPoints[i+1]; j--) {
							if (mapArray[j,yPoints[i]] == PATHVERT) {
								mapArray[j,yPoints[i]] = PATHCROSS;
							} else {
								mapArray[j,yPoints[i]] = PATHHORIZ;
							}
							seqArray[j,yPoints[i]] = i+1;
						}
						mapArray[xPoints[i+1],yPoints[i+1]] = PATHLEFT;
					}
				}
			}
			mapArray[xPoints[0],yPoints[0]] = START;
			mapArray[xPoints[xPoints.Count - 1],yPoints[xPoints.Count - 1]] = END;
			seqArray[xPoints[xPoints.Count - 1],yPoints[xPoints.Count - 1]] = xPoints.Count;
		}
		
		static void textureNodes(int blockRatio, List<int> xPoints, List<int> yPoints, int[,] mapArray, int width, int length, bool firstPathVert) {
			for (int i = 1; i < xPoints.Count - 1; i++) {
				if (mapArray[xPoints[i],yPoints[i]] == PATHDOWN) {
					if (yPoints[i] + 1 >= length) {
						mapArray[xPoints[i],yPoints[i]] = GROUND;
					} else if (mapArray[xPoints[i],yPoints[i]+1] == ICE) {
						if (Server.Math.Rand(0, 100) < blockRatio) {
							mapArray[xPoints[i],yPoints[i]+1] = BLOCK;
						} else {
							mapArray[xPoints[i],yPoints[i]] = GROUND;
						}
					} else if (mapArray[xPoints[i],yPoints[i]+1] != BLOCK) {
						mapArray[xPoints[i],yPoints[i]] = GROUND;
					}
				} else if (mapArray[xPoints[i],yPoints[i]] == PATHLEFT) {
					if (xPoints[i] - 1 < 0) {
						mapArray[xPoints[i],yPoints[i]] = GROUND;
					} else if (mapArray[xPoints[i]-1,yPoints[i]] == ICE) {
						if (Server.Math.Rand(0, 100) < blockRatio) {
							mapArray[xPoints[i]-1,yPoints[i]] = BLOCK;
						} else {
							mapArray[xPoints[i],yPoints[i]] = GROUND;
						}
					} else if (mapArray[xPoints[i]-1,yPoints[i]] != BLOCK) {
						mapArray[xPoints[i],yPoints[i]] = GROUND;
					}
				} else if (mapArray[xPoints[i],yPoints[i]] == PATHUP) {
					if (yPoints[i] - 1 < 0) {
						mapArray[xPoints[i],yPoints[i]] = GROUND;
					} else if (mapArray[xPoints[i],yPoints[i]-1] == ICE) {
						if (Server.Math.Rand(0, 100) < blockRatio) {
							mapArray[xPoints[i],yPoints[i]-1] = BLOCK;
						} else {
							mapArray[xPoints[i],yPoints[i]] = GROUND;
						}
					} else if (mapArray[xPoints[i],yPoints[i]-1] != BLOCK) {
						mapArray[xPoints[i],yPoints[i]] = GROUND;
					}
				} else if (mapArray[xPoints[i],yPoints[i]] == PATHRIGHT) {
					if (xPoints[i] + 1 >= width) {
						mapArray[xPoints[i],yPoints[i]] = GROUND;
					} else if (mapArray[xPoints[i]+1,yPoints[i]] == ICE) {
						if (Server.Math.Rand(0, 100) < blockRatio) {
							mapArray[xPoints[i]+1,yPoints[i]] = BLOCK;
						} else {
							mapArray[xPoints[i],yPoints[i]] = GROUND;
						}
					} else if (mapArray[xPoints[i]+1,yPoints[i]] != BLOCK) {
						mapArray[xPoints[i],yPoints[i]] = GROUND;
					}
				}
				
			}
		}
		
		static void checkBranches(int index, int blockRatio, int security,
				int width, int length, List<int> xPoints, List<int> yPoints,
				int[,] mapArray, int[,] seqArray, List<int> nodeDirs) {
	
			for (int i = (nodeDirs[index] + 1) % 4; i != nodeDirs[index]; i = (i + 1) % 4) {
				if (index == 0 || i != (nodeDirs[index-1] + 2) % 4) { // make sure we don't check a direction we're backtracking on
					bool shortcut = secureBranch(xPoints[index], yPoints[index], i, 0, blockRatio,
					security, width, length, xPoints, yPoints,
					mapArray, seqArray, nodeDirs);
				}
			}
		}
		
		static void addExtraBranches(int index, int branchChance, int branchDepth, int blockRatio,
				int security, int width, int length, List<int> xPoints, List<int> yPoints,
				int[,] mapArray, int[,] seqArray, List<int> nodeDirs) {
			for (int i = (nodeDirs[index] + 1) % 4; i != nodeDirs[index]; i = (i + 1) % 4) {
				if (index == 0 || i != (nodeDirs[index-1] + 2) % 4) { // make sure we don't check a direction we're backtracking on
					if (Server.Math.Rand(0, 100) < branchChance) {
						secureExtraBranch(xPoints[index], yPoints[index], i, branchDepth, blockRatio,
					security, width, length, xPoints, yPoints,
					mapArray, seqArray, nodeDirs);
					}
				}
			}
		}
		
		static bool secureBranch(int x, int y, int dir, int branchDepth, int blockRatio,
				int security, int width, int length, List<int> xPoints, List<int> yPoints,
				int[,] mapArray, int[,] seqArray, List<int> nodeDirs) {
			int distance = dirNodeDistance(x, y, dir, width, length,
			xPoints, yPoints, mapArray, seqArray, nodeDirs);
			bool secured = true;
			if (distance > 0) {
				//decrement branch depth for next recursion
				//shortcut detected; must be blocked
				if (distance == 1) {
					secured = false;
				} else {
					secured = false;
					//if a shortcut is detected, block it off with minimal extra branching
					if (Server.Math.Rand(0, 100) < security) {
						//depending on security toughness, try to seal off the branch before it becomes complicated
						secured = addBranchNode(x, y, dir, branchDepth-1, 0, seqArray[x,y] + 1, blockRatio, true,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
					}
					if (!secured) {
						secured = addRandBranchNode(x, y, dir, branchDepth-1, distance, seqArray[x,y] + 1, blockRatio, 100,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
					}
					//}
				}
			} else if (distance < 0) {
				int ground = ICE;
				if (dir == 0) {
					if (y-distance < length) {
						ground = mapArray[x,y-distance];
					} else {
						ground = VOID;
					}
				} else if (dir == 1) {
					if (x+distance > 0) {
						ground = mapArray[x+distance,y];
					} else {
						ground = VOID;
					}
				} else if (dir == 2) {
					if (y+distance > 0) {
						ground = mapArray[x,y+distance];
					} else {
						ground = VOID;
					}
				} else if (dir == 3) {
					if (x-distance < width) {
						ground = mapArray[x-distance,y];
					} else {
						ground = VOID;
					}
				}
				if (ground == BLOCK && distance < -1) {
					//decrement branch depth for next recursion
					//block detected; make sure it doesn't lead to a shortcut
					secured = addBranchNode(x, y, dir, branchDepth-1, -distance - 1, seqArray[x,y] + 1, blockRatio, true,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
		
					//if it does, block it off with as minimal extra branching as possible
					if (!secured && Server.Math.Rand(0, 100) < security) {
						//depending on security toughness, try to seal off the branch before it becomes complicated
						secured = addBranchNode(x, y, dir, branchDepth-1, 0, seqArray[x,y] + 1, blockRatio, true,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
					}
					if (!secured) {
						secured = addRandBranchNode(x, y, dir, branchDepth-1, -distance, seqArray[x,y] + 1, blockRatio, 100,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
					}
					//if (!secured) distance = -distance;
				} else {
					if (branchDepth > 0 && distance < -1) {
						secured = addRandBranchNode(x, y, dir, branchDepth-1, -distance, seqArray[x,y] + 1, blockRatio, blockRatio,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
		
					}
				}
				//if it leads to a void, it's safe, and do nothing
				//if it leads to a node with earlier age than this, it's a long-bend, and do nothing.
			}
		
			return !secured;
		}
		
		static void secureExtraBranch(int x, int y, int dir, int branchDepth, int blockRatio, int security,
				int width, int length, List<int> xPoints, List<int> yPoints,
				int[,] mapArray, int[,] seqArray, List<int> nodeDirs) {
			int distance = dirNodeDistance(x, y, dir, width, length,
			xPoints, yPoints, mapArray, seqArray, nodeDirs);
			
			//assume that at this point it is impossible to come across a shortcut
		
			if (distance < 0) {
				int ground = ICE;
				if (dir == 0) {
					if (y-distance < length) {
						ground = mapArray[x,y-distance];
					} else {
						ground = VOID;
					}
				} else if (dir == 1) {
					if (x+distance > 0) {
						ground = mapArray[x+distance,y];
					} else {
						ground = VOID;
					}
				} else if (dir == 2) {
					if (y+distance > 0) {
						ground = mapArray[x,y+distance];
					} else {
						ground = VOID;
					}
				} else if (dir == 3) {
					if (x-distance < width) {
						ground = mapArray[x-distance,y];
					} else {
						ground = VOID;
					}
				}
				if (branchDepth > 0 && ground == BLOCK && distance < -1) {
					//decrement branch depth for next recursion
					//block detected; it's been previously checked to be shortcut-proof
					addRandBranchNode(x, y, dir, branchDepth - 1, -distance, seqArray[x,y] + 1, blockRatio, blockRatio,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
					
				} else if (branchDepth > 0 && distance < -1 && ground == VOID) {
					//decrement branch depth for next recursion
					//it doesn't matter if this one failed
					addRandBranchNode(x, y, dir, branchDepth - 1, -distance, seqArray[x,y] + 1, blockRatio, blockRatio,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
				}
			}
		
		}
		
		static bool addRandBranchNode(int x, int y, int dir, int branchDepth, int distance, int age, int blockRatio, int maskRatio,
			int security, int width, int length, List<int> xPoints, List<int> yPoints,
			int[,] mapArray, int[,] seqArray, List<int> nodeDirs) {
			
			
			//add a new node
			//do not block a horizontal or vertical tile
			//do not land on a horizontal or vertical tile that is of a higher age than the selected age
			//try a bunch of times for a legal point
			//if failed, go sequentially from the farthest to the nearest
			//if all that fails, give up
		
			
			bool block = false;
			if (Server.Math.Rand(0, 100) < maskRatio) {
				block = true;
			}
		
			bool succeeded = false;
			int range = 0;
		
			//randomly try 10 times to find a good branch
			for (int i = 0; i < 10; i++) {
				
		
				if (block) {
					range = Server.Math.Rand(0, distance - 1);
				} else {
					range = Server.Math.Rand(1, distance);
				}
			
		
				//add it tentatively and check its implicated branches
				//includes preliminary tests
				
				succeeded = addBranchNode(x, y, dir, branchDepth, range, age, blockRatio, block,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
				
				//if succeeded and no branches make shortcuts, then we're good
				if (succeeded) return true;
			}
		
			if (!succeeded) {
				int farthest = distance - 1;
				if (block) farthest--;
		
				//search actively for a good branch
				for (range = (distance - 1 - farthest); range < farthest; range++) {
					
		
					//add it tentatively and check its implicated branches
					//includes preliminary tests
					
					succeeded = addBranchNode(x, y, dir, branchDepth, range, age, blockRatio, block,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
				
		
					//if succeeded and no branches make shortcuts, then we're good
					if (succeeded) return true;
				}
			}
			
			return false;
		}
		
		static bool addBranchNode(int x, int y, int dir, int branchDepth, int range, int age, int blockRatio, bool block,
				int security, int width, int length, List<int> xPoints, List<int> yPoints,
				int[,] mapArray, int[,] seqArray, List<int> nodeDirs) {
		
			//add branch tentatively and check its implicated branches
				bool succeeded = true;
			
				int ground = 0;
				int seq = 0;
		
				
				if (dir == 0) {
					ground = mapArray[x,y+range];
					seq = seqArray[x,y+range];
				} else if (dir == 1) {
					ground = mapArray[x-range,y];
					seq = seqArray[x-range,y];
				} else if (dir == 2) {
					ground = mapArray[x,y-range];
					seq = seqArray[x,y-range];
				} else if (dir == 3) {
					ground = mapArray[x+range,y];
					seq = seqArray[x+range,y];
				}
		
				
				if ((ground >= PATHVERT && ground <= PATHRIGHT) && seq > age) {
					//if the selected node point laid on a pathway that would make a shortcut...
					return false;
				}
				if (block) {
					if (dir == 0) {
						ground = mapArray[x,y+range+1];
					} else if (dir == 1) {
						ground = mapArray[x-range-1,y];
					} else if (dir == 2) {
						ground = mapArray[x,y-range-1];
					} else if (dir == 3) {
						ground = mapArray[x+range+1,y];
					}
		
					if (ground >= PATHVERT && ground <= PATHRIGHT) {
						return false;
					}
				}
		
				//passed preliminary tests; add branch and check sub-branches
		
					int newX = x;
					int newY = y;
					int oldGround = ICE;
					int oldGround2 = ICE;
					List<int> oldSeq = new List<int>();
		
					//draw branch on map
					if (dir == 0) {
						for (int i = 1; i < range; i++) {
							oldSeq.Add(seqArray[x,y+i]);
							seqArray[x,y+i] = age - 1;
						}
						if (range > 0) {
							oldSeq.Add(seqArray[x,y+range]);
							seqArray[x,y+range] = age;
						}
						if (block) {
							oldGround2 = mapArray[x,y+range+1];
							mapArray[x,y+range+1] = BLOCK;
							oldGround = mapArray[x,y+range];
							if (oldGround != START && oldGround != GROUND) {
								mapArray[x,y+range] = PATHDOWN;
							}
						} else {
							oldGround = mapArray[x,y+range];
							if (oldGround != START) {
								mapArray[x,y+range] = GROUND;
							}
						}
						newY += range;
					} else if (dir == 1) {
						for (int i = 1; i < range; i++) {
							oldSeq.Add(seqArray[x-i,y]);
							seqArray[x-i,y] = age - 1;
						}
						if (range > 0) {
							oldSeq.Add(seqArray[x-range,y]);
							seqArray[x-range,y] = age;
						}
						if (block) {
							oldGround2 = mapArray[x-range-1,y];
							mapArray[x-range-1,y] = BLOCK;
							oldGround = mapArray[x-range,y];
							if (oldGround != START && oldGround != GROUND) {
								mapArray[x-range,y] = PATHLEFT;
							}
						} else {
							oldGround = mapArray[x-range,y];
							if (oldGround != START) {
								mapArray[x-range,y] = GROUND;
							}
						}
						newX -= range;
					} else if (dir == 2) {
						for (int i = 1; i < range; i++) {
							oldSeq.Add(seqArray[x,y-i]);
							seqArray[x,y-i] = age - 1;
						}
						if (range > 0) {
							oldSeq.Add(seqArray[x,y-range]);
							seqArray[x,y-range] = age;
						}
						if (block) {
							oldGround2 = mapArray[x,y-range-1];
							mapArray[x,y-range-1] = BLOCK;
							oldGround = mapArray[x,y-range];
							if (oldGround != START && oldGround != GROUND) {
								mapArray[x,y-range] = PATHUP;
							}
						} else {
							oldGround = mapArray[x,y-range];
							if (oldGround != START) {
								mapArray[x,y-range] = GROUND;
							}
						}
						newY -= range;
					} else if (dir == 3) {
						for (int i = 1; i < range; i++) {
							oldSeq.Add(seqArray[x+i,y]);
							seqArray[x+i,y] = age - 1;
						}
						if (range > 0) {
							oldSeq.Add(seqArray[x+range,y]);
							seqArray[x+range,y] = age;
						}
						if (block) {
							oldGround2 = mapArray[x+range+1,y];
							mapArray[x+range+1,y] = BLOCK;
							oldGround = mapArray[x+range,y];
							if (oldGround != START && oldGround != GROUND) {
								mapArray[x+range,y] = PATHRIGHT;
							}
						} else {
							oldGround = mapArray[x+range,y];
							if (oldGround != START) {
								mapArray[x+range,y] = GROUND;
							}
						}
						newX += range;
					}
		
					//then, if range was greater than 0, MUST check the new node for shortcuts
					if (range > 0) {
						for (int i = (dir + 3) % 4; i != (dir + 2) % 4; i = (i + 1) % 4) {
							bool shortcut = secureBranch(newX, newY, i, branchDepth, blockRatio,
								security, width, length, xPoints, yPoints,
								mapArray, seqArray, nodeDirs);
							//we added a branch whose sub-branches could not be secured
							if (shortcut) {
								succeeded = false;
								break;
							}
						}
					}
					
					//if unsuccessful, undo the changes
					if (!succeeded) {
						if (dir == 0) {
							for (int i = 1; i < range; i++) {
								seqArray[x,y+i] = oldSeq[i-1];
							}
							if (range > 0) {
								seqArray[x,y+range] = oldSeq[range-1];
							}
							if (block) {
								mapArray[x,y+range+1] = oldGround2;
								mapArray[x,y+range] = oldGround;
							} else {
								mapArray[x,y+range] = oldGround;
							}
						} else if (dir == 1) {
							for (int i = 1; i < range; i++) {
								seqArray[x-i,y] = oldSeq[i-1];
							}
							if (range > 0) {
								seqArray[x-range,y] = oldSeq[range-1];
							}
							if (block) {
								mapArray[x-range-1,y] = oldGround2;
								mapArray[x-range,y] = oldGround;
							} else {
								mapArray[x-range,y] = oldGround;
							}
						} else if (dir == 2) {
							for (int i = 1; i < range; i++) {
								seqArray[x,y-i] = oldSeq[i-1];
							}
							if (range > 0) {
								seqArray[x,y-range] = oldSeq[range-1];
							}
							if (block) {
								mapArray[x,y-range-1] = oldGround2;
								mapArray[x,y-range] = oldGround;
							} else {
								mapArray[x,y-range] = oldGround;
							}
						} else if (dir == 3) {
							for (int i = 1; i < range; i++) {
								seqArray[x+i,y] = oldSeq[i-1];
							}
							if (range > 0) {
								seqArray[x+range,y] = oldSeq[range-1];
							}
							if (block) {
								mapArray[x+range+1,y] = oldGround2;
								mapArray[x+range,y] = oldGround;
							} else {
								mapArray[x+range,y] = oldGround;
							}
						}
					}
					
			return succeeded;
		}
		
		static int dirNodeDistance(int x, int y, int dir,
				int width, int length, List<int> xPoints, List<int> yPoints,
				int[,] mapArray, int[,] seqArray, List<int> nodeDirs) {
			//if another node is in this direction and is greater than this node's number + 1 return the distance
			//else return a negative number of how far the check goes
			if (dir == 0) {
				int lim = y+1;
				for (int i = y+1; i < length; i++) {
					if (mapArray[x,i] == BLOCK) {
						if (seqArray[x,i-1] > seqArray[x,y]+1) {
							if (lim == i) {
								return (lim - 1) - y;
							} else {
								return lim - y;
							}
						} else {
							return y - lim;
						}
					} else if (mapArray[x,i] == GROUND || mapArray[x,i] == START || mapArray[x,i] == END) {
						if (seqArray[x,i] > seqArray[x,y] + 1) {
							return lim - y;
						} else {
							return y - lim;
						}
					}
					
					if (mapArray[x,lim] >= PATHDOWN && mapArray[x,lim] <= PATHRIGHT) {
						//do not increment lim
					} else {
						lim++;
					}
				}
				return y - lim;
			} else if (dir == 1) {
				int lim = x-1;
				for (int i = x-1; i >= 0; i--) {
					if (mapArray[i,y] == BLOCK) {
						if (seqArray[i+1,y] > seqArray[x,y]+1) {
							if (lim == i) {
								return x - (lim + 1);
							} else {
								return x - lim;
							}
						} else {
							return lim - x;
						}
					} else if (mapArray[i,y] == GROUND || mapArray[i,y] == START || mapArray[i,y] == END) {
						if (seqArray[i,y] > seqArray[x,y] + 1) {
							return x - lim;
						} else {
							return lim - x;
						}
					}
		
					if (mapArray[lim,y] >= PATHDOWN && mapArray[lim,y] <= PATHRIGHT) {
						//do not increment lim
					} else {
						lim--;
					}
		
				}
				return lim - x;
			} else if (dir == 2) {
				int lim = y-1;
				for (int i = y-1; i >= 0; i--) {
					if (mapArray[x,i] == BLOCK) {
						if (seqArray[x,i+1] > seqArray[x,y]+1) {
							if (lim == i) {
								return y - (lim + 1);
							} else {
								return y - lim;
							}
						} else {
							return lim - y;
						}
					} else if (mapArray[x,i] == GROUND || mapArray[x,i] == START || mapArray[x,i] == END) {
						if (seqArray[x,i] > seqArray[x,y] + 1) {
							return y - lim;
						} else {
							return lim - y;
						}
					}
		
					if (mapArray[x,lim] >= PATHDOWN && mapArray[x,lim] <= PATHRIGHT) {
						//do not increment lim
					} else {
						lim--;
					}
				}
				return lim - y;
			} else if (dir == 3) {
				int lim = x+1;
				for (int i = x+1; i < width; i++) {
					if (mapArray[i,y] == BLOCK) {
						if (seqArray[i-1,y] > seqArray[x,y]+1) {
							if (lim == i) {
								return (lim - 1) - x;
							} else {
								return lim - x;
							}
						} else {
							return x - lim;
						}
					} else if (mapArray[i,y] == GROUND || mapArray[i,y] == START || mapArray[i,y] == END) {
						if (seqArray[i,y] > seqArray[x,y] + 1) {
							return lim - x;
						} else {
							return x - lim;
						}
					}
		
					if (mapArray[lim,y] >= PATHDOWN && mapArray[lim,y] <= PATHRIGHT) {
						//do not increment lim
					} else {
						lim++;
					}
				}
				return x - lim;
			}
			
		
			return 0;
		}
		
		#endregion
		
		#region Ice Path
		static void GenIcePath(int width, int length, int bends, List<int> xPoints, List<int> yPoints, ref bool firstPathVert, bool stableStart, bool stableEnd) {
			
			int startX = 0;
			int startY = 0;
			
			if (stableStart) {
				if (Server.Math.Rand(0, 2) == 0) {
					if (Server.Math.Rand(0, 2) == 0) {
						startX = width - 1;
					} else {
						startX = 0;
					}
					startY = Server.Math.Rand(0, length);
				} else {
					startX = Server.Math.Rand(0, width);
					if (Server.Math.Rand(0, 2) == 0) {
						startY = length - 1;
					} else {
						startY = 0;
					}
				}
			} else {
				startX = Server.Math.Rand(0, width);
				startY = Server.Math.Rand(0, length);
			}
			
			xPoints.Add(startX);
			yPoints.Add(startY);
		
			int endX = 0, endY = 0;
		
			bool endFound = false;
			for (int i = 0; i < 100; i++) {
				if (stableEnd) {
					if (Server.Math.Rand(0, 2) == 0) {
						if (Server.Math.Rand(0, 2) == 0) {
							endX = width - 1;
						} else {
							endX = 0;
						}
						endY = Server.Math.Rand(0, length);
					} else {
						endX = Server.Math.Rand(0, width);
						if (Server.Math.Rand(0, 2) == 0) {
							endY = length - 1;
						} else {
							endY = 0;
						}
					}
				} else {
					endX = Server.Math.Rand(0, width);
					endY = Server.Math.Rand(0, length);
				}
		
				if (endX == startX && endY - 1 <= startY && endY + 1 >= startY ||
					endY == startY && endX - 1 <= startX && endX + 1 >= startX) {
					
				} else {
					endFound = true;
					break;
				}
			}
		
			if (endFound) {
				xPoints.Add(endX);
				yPoints.Add(endY);
		
				if (startX == endX) {
					firstPathVert = true;
				} else if (startY == endY) {
					firstPathVert = false;
				} else {
					if (Server.Math.Rand(0, 2) == 0) {
						//use startX, endY as midpoint
						xPoints.Insert(1, startX);
						yPoints.Insert(1, endY);
						firstPathVert = true;
					} else {
						//use endX, startY as midpoint
						xPoints.Insert(1, endX);
						yPoints.Insert(1, startY);
						firstPathVert = false;
					}
				}
		
				
				for (int i = 0; i < bends * 10 && xPoints.Count - 2 < bends; i++) {
		
					int random = Server.Math.Rand(0, xPoints.Count);
					addBend(random, length, width, xPoints, yPoints, ref firstPathVert);
				}
			}
		
		}
		
		static void addBend(int index, int length, int width, List<int> xPoints, List<int> yPoints, ref bool firstPathVert) {
			int indexDir;
			
			if (index == 0) {
				//index 0 must be end-hooked
				indexDir = 1;
			} else if (index == xPoints.Count - 1) {
				//index (count - 1) must be start-hooked
				indexDir = -1;
			} else {
				//choose whether to be end-hooked or start-hooked
				if (Server.Math.Rand(0, 2) == 0) {
					//end hooked
					indexDir = 1;
				} else {
					//start hooked
					indexDir = -1;
				}
			}
			
			bool oldFirstVert = firstPathVert;
		
			int newX = xPoints[index];
			int newY = yPoints[index];
			if ((index % 2 == 0) != (firstPathVert != (indexDir == 1))) {
				//(truth-table checked; true for all cases in which the points are on a vertical line)
				newY = choosePointBetween(yPoints[index+indexDir], yPoints[index]);
			} else {
				newX = choosePointBetween(xPoints[index+indexDir], xPoints[index]);
			}
		
			
			//int err = 0;
			if (newX != xPoints[index+indexDir] || newY != yPoints[index+indexDir]) {
				//if the second point IS NOT an already existing point, add the new point
		
				if (indexDir == 1) {
					//insert at index + 1
					xPoints.Insert(index + 1, newX);
					yPoints.Insert(index + 1, newY);
					//twice
					xPoints.Insert(index + 1, newX);
					yPoints.Insert(index + 1, newY);
					//err = 1;
				} else {
					//insert at index
					xPoints.Insert(index, newX);
					yPoints.Insert(index, newY);
					//twice
					xPoints.Insert(index, newX);
					yPoints.Insert(index, newY);
					//2 new nodes were added before index; index must be incremented
					index += 2;
					//err = 2;
				}
			} else {
				//if the second point IS an already existing point
		
				if (index + indexDir == 0) {
					//if the second point is the beginning, then add one extra node
					xPoints.Insert(0, newX);
					yPoints.Insert(0, newY);
					//isFirstPathVert must also be changed
					firstPathVert = !firstPathVert;
					//a new node was added before index; index must be incremented
					index++;
					//err = 3;
				} else if (index + indexDir == xPoints.Count - 1) {
					//if the second point is the end, then add one extra node
					xPoints.Add(newX);
					yPoints.Add(newY);
					//err = 4;
				}
			}
		
			if (index == 0) {
				//if the first point is at the very beginning,
				//add a node before it to keep the path taped to the first point
				int addedX = xPoints[0];
				int addedY = yPoints[0];
				xPoints.Insert(0, addedX);
				yPoints.Insert(0, addedY);
				//isFirstPathVert must also be changed
				firstPathVert = !firstPathVert;
				//a new node was added before index; index must be incremented
				index++;
				//err += 10;
			} else if (index == xPoints.Count - 1) {
				//if the first point is at the very end,
				//add a node after it to keep the path taped to the end point
				int addedX = xPoints[xPoints.Count - 1];
				int addedY = yPoints[yPoints.Count - 1];
				xPoints.Add(addedX);
				yPoints.Add(addedY);
				//err += 20;
			}
		
			
			int oldX1 = xPoints[index];
			int oldY1 = yPoints[index];
		
			int oldX2 = xPoints[index+indexDir];
			int oldY2 = yPoints[index+indexDir];
		
		
			//try an arbitrary amount of times to move the new bend
			//without it intersecting any existing lines
			bool succeeded = false;
			for (int i = 0; i < 10; i++) {
				moveLineBetween(index, indexDir, length, width, xPoints, yPoints, ref firstPathVert);
				if (!doesMoveOverlap(index, indexDir, length, width, xPoints, yPoints, ref firstPathVert) &&
				!isNodeAdjacent(index, length, width, xPoints, yPoints, ref firstPathVert) &&
				!isNodeAdjacent(index+indexDir, length, width, xPoints, yPoints, ref firstPathVert) &&
				!isNodeAdjacent(index+2*indexDir, length, width, xPoints, yPoints, ref firstPathVert)) {
					if (!isNodeAdjacent(0, length, width, xPoints, yPoints, ref firstPathVert) &&
					!isNodeAdjacent(xPoints.Count - 1, length, width, xPoints, yPoints, ref firstPathVert)) {
						succeeded = true;
						break;
					}
				}
			}
			if (!succeeded) {
				xPoints[index] = oldX1;
				yPoints[index] = oldY1;
		
				xPoints[index+indexDir] = oldX2;
				yPoints[index+indexDir] = oldY2;
				
				//remove redundant points
				removePointOverlap(index, length, width, xPoints, yPoints, ref firstPathVert);
				int index2 = index+indexDir;
				removePointOverlap(index2, length, width, xPoints, yPoints, ref firstPathVert);
		
				//change back to old vertical pattern
				firstPathVert = oldFirstVert;
			} else {
				
			}
		}
		
		static int choosePointBetween(int X1, int X2) {
			
			//chooses a random number between the first number (inclusive) and the second number (exclusive)
			//X1 can be larger OR smaller than X2
			int random;
			if (X1 < X2) {
				random = Server.Math.Rand(X1, X2);
			} else {
				random = Server.Math.Rand(X2+1, X1+1);;
			}
		
			return random;
		}
		
		static void moveLineBetween(int index, int indexDir, int length, int width, List<int> xPoints, List<int> yPoints, ref bool firstPathVert) {
			
			if ((index % 2 == 0) != (firstPathVert != (indexDir == 1))) {
				//(truth-table checked; true for all cases in which the points are on a vertical line)
				int newX = Server.Math.Rand(0, width);
				if (newX == xPoints[index]) {
					if (newX > 0) {
						newX--;
					} else {
						newX++;
					}
				}
				xPoints[index] = newX;
				xPoints[index+indexDir] = newX;
			} else {
				int newY = Server.Math.Rand(0, length);
				if (newY == yPoints[index]) {
					if (newY > 0) {
						newY--;
					} else {
						newY++;
					}
				}
				yPoints[index] = newY;
				yPoints[index+indexDir] = newY;
			}
		}
		
		static void removePointOverlap(int index, int length, int width, List<int> xPoints, List<int> yPoints, ref bool firstPathVert) {
			bool intersected = false;
			for (int i = xPoints.Count - 1; i >= 0; i--) {
				if (xPoints[i] == xPoints[index] && yPoints[i] == yPoints[index] && i != index) {
					if (i < index) index--;
					//erase one point
					xPoints.RemoveAt(i);
					yPoints.RemoveAt(i);
					intersected = true;
				}
			}
		
			//erase the original point if it intersected, and it's not on an edge
			if (intersected && index > 0 && index < xPoints.Count - 1) {
				xPoints.RemoveAt(index);
				yPoints.RemoveAt(index);
			}
		}
		
		
		static bool doesMoveOverlap(int index, int indexDir, int length, int width, List<int> xPoints, List<int> yPoints, ref bool firstPathVert) {
			
			if (indexDir == 1) {
				if (doesLineOverlap(index, length, width, xPoints, yPoints, ref firstPathVert)) return true;
				if (index - 1 >= 0 && doesLineOverlap(index-1, length, width, xPoints, yPoints, ref firstPathVert)) return true;
				if (index + 2 < xPoints.Count && doesLineOverlap(index+1, length, width, xPoints, yPoints, ref firstPathVert)) return true;
			} else {
				if (doesLineOverlap(index-1, length, width, xPoints, yPoints, ref firstPathVert)) return true;
				if (index - 2 >= 0 && doesLineOverlap(index-2, length, width, xPoints, yPoints, ref firstPathVert)) return true;
				if (index + 1 < xPoints.Count && doesLineOverlap(index, length, width, xPoints, yPoints, ref firstPathVert)) return true;
			}
			
			return false;
		}
		
		static bool doesLineOverlap(int index, int length, int width, List<int> xPoints, List<int> yPoints, ref bool firstPathVert) {
			int start = index % 2;
			
			//check with every other line
			for (int i = start; i < xPoints.Count - 1; i += 2) {
				if (i != index) {
					if (firstPathVert == (start == 0)) {
						//if we're dealing with vertical lines
						if (xPoints[i] == xPoints[index]) {
							if (yPoints[index] >= yPoints[i] && yPoints[index] <= yPoints[i+1]
		                        || yPoints[index] >= yPoints[i+1] && yPoints[index] <= yPoints[i]
		                        || yPoints[index+1] >= yPoints[i] && yPoints[index] <= yPoints[i+1]
		                        || yPoints[index+1] >= yPoints[i+1] && yPoints[index] <= yPoints[i]
		                        || yPoints[i] >= yPoints[index] && yPoints[i] <= yPoints[index+1]
		                        || yPoints[i] >= yPoints[index+1] && yPoints[i] <= yPoints[index]
		                        || yPoints[i+1] >= yPoints[index] && yPoints[i] <= yPoints[index+1]
		                        || yPoints[i+1] >= yPoints[index+1] && yPoints[i] <= yPoints[index]) {
									return true;
							}
						}
					} else {
						//if we're dealing with horizontal lines
						if (yPoints[i] == yPoints[index]) {
							if (xPoints[index] >= xPoints[i] && xPoints[index] <= xPoints[i+1]
		                        || xPoints[index] >= xPoints[i+1] && xPoints[index] <= xPoints[i]
		                        || xPoints[index+1] >= xPoints[i] && xPoints[index] <= xPoints[i+1]
		                        || xPoints[index+1] >= xPoints[i+1] && xPoints[index] <= xPoints[i]
		                        || xPoints[i] >= xPoints[index] && xPoints[i] <= xPoints[index+1]
		                        || xPoints[i] >= xPoints[index+1] && xPoints[i] <= xPoints[index]
		                        || xPoints[i+1] >= xPoints[index] && xPoints[i] <= xPoints[index+1]
		                        || xPoints[i+1] >= xPoints[index+1] && xPoints[i] <= xPoints[index]) {
									return true;
							}
						}
					}
				}
			}
		
			//check on endpoints
			
			if (firstPathVert == (start == 0)) {
				//if we're dealing with vertical lines
				if (xPoints[0] == xPoints[index]) {
					if (yPoints[index] >= yPoints[0] && yPoints[index+1] <= yPoints[0]
						|| yPoints[index+1] >= yPoints[0] && yPoints[index] <= yPoints[0]) {
							if (index != 0) return true;
					}
				}
				if (xPoints[xPoints.Count-1] == xPoints[index]) {
					if (yPoints[index] >= yPoints[xPoints.Count-1] && yPoints[index+1] <= yPoints[xPoints.Count-1]
						|| yPoints[index+1] >= yPoints[xPoints.Count-1] && yPoints[index] <= yPoints[xPoints.Count-1]) {
							if (index != xPoints.Count-2) return true;
					}
				}
			} else {
				//if we're dealing with horizontal lines
				if (yPoints[0] == yPoints[index]) {
					if (xPoints[index] >= xPoints[0] && xPoints[index+1] <= xPoints[0]
						|| xPoints[index+1] >= xPoints[0] && xPoints[index] <= xPoints[0]) {
							if (index != 0) return true;
					}
				}
				if (yPoints[yPoints.Count-1] == yPoints[index]) {
					if (xPoints[index] >= xPoints[yPoints.Count-1] && xPoints[index+1] <= xPoints[yPoints.Count-1]
						|| xPoints[index+1] >= xPoints[yPoints.Count-1] && xPoints[index] <= xPoints[yPoints.Count-1]) {
							if (index != xPoints.Count-2) return true;
					}
				}
			}
			
			return false;
		}
		
		
		static bool isNodeAdjacent(int index, int length, int width, List<int> xPoints, List<int> yPoints, ref bool firstPathVert) {
			//needed to check for adjacency shortcuts
			//does not count for nodes that are next to each other in sequence
			for (int i = 0; i < xPoints.Count; i++) {
				if (index != i && index - 1 != i && index + 1 != i) {
					if (xPoints[i] == xPoints[index]) {
						if (yPoints[i] == yPoints[index] - 1 || yPoints[i] == yPoints[index] + 1) return true;
					} else if (yPoints[i] == yPoints[index]) {
						if (xPoints[i] == xPoints[index] - 1 || xPoints[i] == xPoints[index] + 1) return true;
					}
				}
			}
		
			return false;
		}
		
		
		#endregion
	}
}