# __DESCRIPTION__ :

This is our own Unity Health System package. Please take note that even through we made it public, it is intended to our own projects, therefore might not fit yours.

It includes: 

- A Health System
- Tick Damages and their handler
- Custom Damages calculator order
- Death, Damages, Heal callbacks
- Health & Heal customizable popups
- Knockback

## __TICK DAMAGES__ :
The Tick Damages will embark different datas
![image](https://github.com/BalD1/com.stdnounou.health-system/assets/24933826/cc70dda9-4780-4fa2-82ee-cf823137f2e2)

- ID : The tick damage's ID will be used to retrieve it inside of the Health System when applied
- Damages Type : WIP
- Attributes : [*from our StatsSystem Package*](https://github.com/BalD1/com.stdnounou.stats-system) ; Used to give customizable modifiers depending on the receiver's attributes. (Ex : Fire att. deals 2x damages to plant att.)
- Stackable : Can the receiver have more than one of this TD at the same time ?
- Chances to apply : Straightforward. When sent, with how much %chances this TD should apply ?
- Ticks Lifetime : How many lifetime ticks should this have ? (By default, one tick is .25s. So 4 ticks = 1s)
- Required Ticks To Trigger : How much ticks should pass before triggering the TD ?
- Damages : Can have 3 types, and a amount.
  - Fixed : The damages amount will not change, regardless of the sender's damages stat
  - Additive : Adds the amount to the sender's damages stat
  - Multiplier : Multiplies the amount to the sender's damages stat
- Crit Chances : How much %crit should the TD have ?
  - The types works the same as above, but for the crit chances stat.
- Crit Multiplier : In case of critical hit, by how much should we multiply the damages ?
  - The types works the same as above, but for the crit multiplier stat.
- Particles : You can give the TD particles to play once applied.

## __CALCULATORS__ :
The calculators will let you customize the damages calculation.
![image](https://github.com/BalD1/com.stdnounou.health-system/assets/24933826/e90e2b20-ed86-4146-8c4a-7e07ccaee6d5)
This is the default calculator.

- Base Damage Calculator : The order of modifiers calculation for the sender's damages stat
![image](https://github.com/BalD1/com.stdnounou.health-system/assets/24933826/6f68601d-f48b-4118-8d4a-549b010c424b)
Here :
  - The calculator will start by calculating the receiver's Affiliation modifiers against the sender's
  - Then, the sender's Affiliation modifiers against the receiver's
  - Finally, the receiver's Attributes modifiers

Once the Base Damages stat has been calculated, the calculator will move on to the next steps
![image](https://github.com/BalD1/com.stdnounou.health-system/assets/24933826/c4ade563-dda5-4925-88d2-cbaeb4cf819d)
Here; 
- The calculator will first roll a Crit, if the crit triggered, apply the multiplier
- Then apply any Damages Reductions available.

You can add any calculations you wish, or move their order, and give any Health System it's own damages calculator.

## __HEALTH TEXT POPUP__ :
The Health Text Popup is an additional module that will display the lost or gained health.    
![gif](https://github.com/BalD1/com.stdnounou.health-system/assets/24933826/3bba4baf-afc5-4a25-837f-5c359b3e22cf)    
![image](https://github.com/BalD1/com.stdnounou.health-system/assets/24933826/389c96ff-c694-4f33-bc73-a75012d1ef58)

It is based on our TextPopup Component, and you can give them custom Data.    
![image](https://github.com/BalD1/com.stdnounou.health-system/assets/24933826/7a149517-8694-4870-b1f7-ebf8cdc1f71a)


# __DEPENDECIES__ :

-StdNounou custom core : https://github.com/BalD1/com.stdnounou.unity-custom-core    
-StdNounou tick manager : https://github.com/BalD1/com.stdnounou.unity-tick-manager    
-StdNounou Stats System : https://github.com/BalD1/com.stdnounou.stats-system    
-Ayellowpaper's Serialized Dictionary: https://github.com/ayellowpaper/SerializedDictionary.git    
