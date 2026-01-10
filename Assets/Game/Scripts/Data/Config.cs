using System;
using System.Collections.Generic;
using UnityEngine.Analytics;

/// <summary>配置</summary>
public static class Config
{
    public static bool isLoading = true;
    public static int ClearType=1;
    public static string[] passName = new string[] { "1-1", "1-2", "1-3", "1-4", "2-1", "2-2", "2-3", "3-1", "3-2", "3-3", "3-4", "4-1", "4-2", "4-3", "4-4" };
    public static int chainCount;
    public static int passIndex = 0;
}                                                                    
                                      
/// <summary>事件合集 </summary>               
public enum Events                                          
{                                                                   
    None,                                                         
    OnChangeLife,
    OnModVideoPlayStart,
    OnModVideoPlayEnd, 
    OnLazzerHit,
    NpcTalkShow,
}                                                                  
// 移动方向                                                 
public enum MoveDirection                                
{                                                                     
    Left,                                                           
    Right
}
public enum MoveType
{
    Normal,
    HighLeft,
    HighRight,
    MaxLeft,
    MaxRight
}