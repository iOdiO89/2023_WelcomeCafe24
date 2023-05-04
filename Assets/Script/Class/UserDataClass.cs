using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataClass
{
    public int gold;
    public int reputation;
    public int day;

    public List<bool> ingredientUnlock; // 재료 해금여부
    public List<int> recipeUnlock; // 레시피 해금여부
    public List<bool> machineUnlock; // 기계 해금여부
    public int machineLevel; // 상점에 등장하는 기기의 레벨(보급, 고급, 하이엔드)

    public float bgmVolume;
    public float effectVolume;


    public UserDataClass(){
        gold = 10000;
        reputation = 0;
        day = 0;

        ingredientUnlock = new List<bool>();
        ingredientUnlock.Clear();
        for(int i=0; i<14; i++){
            switch(i){
                case 0: // 물
                case 1: // 우유
                case 4: // 에스프레소
                case 9: // 민트
                    ingredientUnlock.Add(true);
                    break;
                default:
                    ingredientUnlock.Add(false);
                    break;
            }
        }

        recipeUnlock = new List<int>();
        recipeUnlock.Clear();
        for(int i=0; i<21; i++){
            switch(i){
                case 0: // 에스프레소
                case 1: // 아메리카노
                case 15: // 민트 티
                    recipeUnlock.Add(1);
                    break;
                default:
                    recipeUnlock.Add(0);
                    break;
            } 
        }

        machineUnlock = new List<bool>();
        machineUnlock.Clear();
        for(int i=0; i<7; i++){
            if(i<3){
                machineUnlock.Add(true);
            }
            else{
                machineUnlock.Add(false);
            }
            
        }

        machineLevel = 1;

        bgmVolume = 0.4f;
        effectVolume = 0.7f;

    }

}