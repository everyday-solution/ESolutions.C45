#ifndef __STATS_H__
 #define __STATS_H__
//Variables
static float Val[] = {  0,  0.001, 0.005, 0.01, 0.05, 0.10, 0.20, 0.40, 1.00},
      Dev[] = {4.0,  3.09,  2.58,  2.33, 1.65, 1.28, 0.84, 0.25, 0.00};

//Prototypes
extern float AddErrs (ItemCount N, ItemCount e);

#endif //__STATS_H__