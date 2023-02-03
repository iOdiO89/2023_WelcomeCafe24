using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataClass
{
    public int gold;
    public int reputation;

    public List<bool> ingredientUnlock; // 재료 해금여부
    public List<bool> recipeUnlock; // 레시피 해금여부
    public List<bool> machineUnlock; // 기계 해금여부

    public UserDataClass(){
        gold = 0;
        reputation = 0;

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

        recipeUnlock = new List<bool>();
        recipeUnlock.Clear();
        for(int i=0; i<21; i++){
            switch(i){
                case 0: // 에스프레소
                case 1: // 아메리카노
                case 15: // 민트 티
                    recipeUnlock.Add(true);
                    break;
                default:
                    recipeUnlock.Add(false);
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

    }

}