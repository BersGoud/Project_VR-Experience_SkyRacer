# Sky Racer

## Inleiding

Voor de VR-Experience cursus kregen we de opdracht om een VR-spel te maken. We besloten een spel te maken waarin de speler piloot is van een klein vliegtuig. De speler moet door een reeks ringen vliegen om het niveau te voltooien. In dit document leggen we het proces van het maken van dit spel uit.

## Methodologie

### Tools

Voor dit project hebben we Unity gebruikt om het spel te maken. We gebruikten Oculus Quest 2 om het spel te testen. We gebruikten de volgende assets:

#### ML-Agents Toolkit

- Unity ML-Agents Toolkit via de Unity Package Manager of GitHub.

#### VR SDK's

- culus Integration of SteamVR SDK.

#### ML-Agents Specifieke Assets

- Brain Prefabs: Voorgeconfigureerde ML-agent brains (Decision Requester, Behavior Parameters).
- Training Configuration Files: YAML-bestanden voor het configureren van trainingsparameters.

#### VR Specifieke Assets

- Controller Models en Scripts: Voor de interactie met de VR-wereld.
- VR Camera Rigs: Voor het instellen van de VR-camera en het gezichtsveld.
- Teleportation en Movement Scripts: Voor verplaatsing binnen de VR-omgeving.

### De gameplay loop

De gameplay loop bestaat uit het kunnen vliegen door checkpoints. Wanneer de speler door een checkpoint vliegt, kan hij naar de volgende gaan. Wanneer de speler crasht, wordt hij een paar meter boven de grond geteleporteerd. De speler kan het niveau ook opnieuw starten door op een knop in het pauzemenu te drukken. Het spel wordt gespeeld tegen een ML-agent die ook probeert door de checkpoints te vliegen, wat een race simuleert.

### Acties

De speler kan de volgende acties uitvoeren:

- Boost (L-trigger)
- Pauzeren/Hervatten (Y-knop)
- Links rollen (R-stick links)
- Rechts rollen (R-stick rechts)
- Links gieren (R-stick omhoog)
- Rechts gieren (R-stick omlaag)

De ML-agent kan de volgende acties uitvoeren:

- Naar links bewegen
- Naar rechts bewegen
- Naar boven bewegen
- Naar beneden bewegen
- Vooruit bewegen
- Achteruit bewegen
- Links draaien
- Rechts draaien

Waarnemingen die de ML-agent kan doen: #TODO Lander kijken

- Positie
- Rotatie
- Positie van het volgende checkpoint
- Afstand tot het volgende checkpoint

De ML-agent wordt getraind met behulp van het PPO-algoritme en heeft de volgende beloningen en straffen:

- Grote beloning voor het vliegen door een checkpoint
- Kleine beloning voor het verkleinen van de afstand tot het checkpoint
- Kleine straf voor het vergroten van de afstand tot het checkpoint
- Straf voor crashen
- Straf voor te ver weg vliegen van het checkpoint
- Straf voor te langzaam vliegen
- Straf voor buiten de grenzen gaan

### Installatie

#### Versiebeheer

Unity: 2022.3.10f1
Anaconda: 1.12.3

- python 3.9.19
- torch 1.7.1+cu110
- mlagents 0.30.0
- mlagents_envs 0.30.0

### Objecten

Het spel bestaat uit de volgende objecten:

- Vliegtuig
  - Dit vliegtuig is een model dat we van Sketchfab hebben gehaald. (Simple Airplane Controller | Game Toolkits | Unity Asset Store, 2023)
- Checkpoints
  - De checkpoints zijn objecten die we in Unity hebben gemaakt. Het zijn ringen waar de speler doorheen moet vliegen. De checkpoints bevatten de volgende scripts:
    - Checkpoint.cs
- Grond
  - De grond is een terrein dat we hebben gemaakt met behulp van de terraintool in Unity.
    - Gebouwen komen uit het WhiteCity asset pack. (White City | 3D Urban | Unity Asset Store, 2018)
- Skybox
  - We gebruikten een skybox van Midgard Skybox. (Midgard Skybox | 2D Sky | Unity Asset Store, 2024)
- UI
  - De UI bevat ...
- ML-agent
- Camera
  - Het camerasysteem dat werd gebruikt, kwam met het vliegtuigmodel. We hebben enkele aanpassingen aan het camerasysteem gedaan om het te laten werken met het spel.
- Lichten
- Audio
  - We gebruikten een aantal audio-bestanden uit verschillende bronnen om het spel realistischer en meeslepender te maken. (Free Checkpoint Sound Effect Download | SFX MP3 Library | Soundsnap, z.d.), (Simple Airplane Controller | Game Toolkits | Unity Asset Store, 2023)

### Overzicht van de observaties, mogelijke acties en beloningen

**Observaties:**

1. Normale rotatiehoeken van het vliegtuig.
2. Relatieve positie en afstand tot het volgende controlepunt.
3. Huidige snelheid van het vliegtuig.
4. **Ray Perceptor Sensor3D**: Gegevens van de omgevingsdetectie door middel van raycasting.

**Mogelijke Acties:**

1. Vooruitbeweging (continue actie tussen 0 en 1).
2. Links/rechts rotatie (continue actie tussen -1 en 1).
3. Verticale beweging (alleen als driedimensionale beweging is ingeschakeld, continue actie tussen -1 en 1).

**Beloningen:**

1. Vermindering van de afstand tot het volgende controlepunt (+0.1 \* afstandsvermindering).
2. Vermindering van de verticale afstand tot het volgende controlepunt (+0.05 \* afstandsvermindering).
3. Bereiken van een controlepunt (+5).
4. Completeren van het circuit (+60).
5. Stilstaan (-0.01).
6. Erratische rotatie (-0.01).
7. Tijdsgebonden straf (-0.001).
8. Botsen met een muur of obstakel (-2).

### Beschrijving van de objecten

1. **Agent (AirplaneAgentGame):** Het door de agent bestuurde vliegtuig met een vaste snelheid, rotatiesnelheid en verticale snelheid.
2. **Rigidbody (rb):** Fysieke component die zorgt voor bewegingsdynamiek van het vliegtuig.
3. **CheckpointManager:** Beheert de controlepunten die het vliegtuig moet bereiken.
4. **Propeller:** Visuele representatie van de propeller die draait tijdens de vlucht.
5. **Ray Perceptor Sensor3D:** Sensor die door middel van raycasting de omgeving scant en obstakels detecteert.

### Gedragingen van de objecten

1. **AirplaneAgentGame:**

   - **Initialize:** Stelt de Rigidbody in en reset controlepunten bij het begin.
   - **OnEpisodeBegin:** Reset de positie, snelheid, en oriëntatie van het vliegtuig.
   - **CollectObservations:** Verzamelt observaties van rotatie, relatieve positie tot het controlepunt, snelheid en gegevens van de Ray Perceptor Sensor3D.
   - **OnActionReceived:** Voert acties uit op basis van ontvangen invoer, inclusief voorwaartse beweging, rotatie en verticale beweging. Geeft beloningen/straf op basis van gedrag en prestaties.
   - **Heuristic:** Handmatige invoer voor testen van acties.

2. **CheckpointManager:**

   - **ResetCheckpointsAgent:** Reset de controlepunten voor de agent.
   - **GetNextCheckpointAgent:** Haalt het volgende controlepunt op.

3. **Propeller:**

   - **RotatePropellers:** Zorgt ervoor dat de propeller draait.
   - **Update:** Roteert de propeller in elke frame update.

4. **Triggers en Colliders:**
   - **OnTriggerEnter:** Controleert op botsingen met controlepunten, muren, en obstakels en geeft passende beloningen of straffen.

### Versie-informatie

Dit is de derde en definitieve versie van de agent. Er is veel werk gestoken in de vorige twee versies, maar uiteindelijk moesten we ze laten vallen omdat de resultaten erg teleurstellend waren.

**Versie 1:**
Agent op basis van hetzetzelfde movement script als de speler. Na heel veel iteraties, tunen van de reward function en veranderen van trainomgeving (in meerdere scenes proberen trainen), hebben we de movement zelf geintegreert in het agent script en versimpelt.

**Versie 2:**
Minder observaties en minder acties, maar door onbekende redenen bleven er problemen optreden tijdens het trainen. We hebben een aparte scene gebouwd voor de agent enkel te leren zich naar een checkpoint te begeven, en na veel trial en error kregen we eindelijk een redelijk resultaat.10f1

**Versie 3:**
De agent terug meer aties en observaties gegeven binnen de werkende trainingsomgeving. Versie 3 hebben we getraint in 3 aparte scenes, om zo gradueel een complexer model te bekomen.

### One-pager

## Resultaten

### Tensorboard

**Model Versie 3:**
Training in de eerste scene verliep stabiel met een geleidelijke groei in rewards die dan afvlakt.

![alt text](<Screenshot 2024-06-18 015334.png>)

De omgeving is ontworpen om de agent een enkele checkpoint te leren pakken de op een willekeurige positie spawnt in 3d space.

![alt text](<Screenshot 2024-06-18 023035.png>)

De training in de 2de scene verliep iets onstabieler, maar de agent kon ook veel meer rewards verdienen. Er is wel een duidelijke verbetering.

![alt text](<Screenshot 2024-06-18 023545-1.png>)

De omgeving is ontwormen om de agent een circuit van checkpoints te laten volgen die op willekeurige hoogtes in een cirkel spawnen. Een episode eindigd pas wanneer het circuit af is, waarna een grote reward wordt gegeven. De agent moet ook wilekeurige obstakels ontwijken.

![alt text](<Screenshot 2024-06-18 024327-1.png>)

De training in de derde scene, de game scene, hebben we gedaan om de agent te trainen op snelheid, en ook rewards geven op snelheid. De training is maar heel kort uitgevoert, sinds het al snel afflakte en de agent snel genoeg is.

![alt text](<Screenshot 2024-06-18 024726-1.png>)

De game scene volgt een circuit, maar de episode eindigd niet na 1 lap, maar na 3.

![alt text](<Screenshot 2024-06-18 025049.png>)

### Opmerkelijke observaties

Dit een een afbeelding van de volledige training

![alt text](<Screenshot 2024-06-18 025332.png>)

Opmerkelijk is de dip op het einde. Toen werd er van de 2de scnene naar de 3de overgezet, en de agent kereeg te weinig max steps om een lap te doen voor de episode eindigde. Ik heb dit tijdens het trainen aangepast, vandaar de dip.

Ook opmerkelijk is het enorme verschil in rewards van de 1e naar de 2de scene, maar het is wel logisch. De agent wordt pas gereset na een hele lap, in plaats van na elk checkpoint.

## Conclusie

Het trainen van een agent om een vliegtuig te besturen is een moeilijke taak die we zeker hebben onderschat. Het is ons echter, met de nodige compromies, gelukt om een agent te trainen die een circuit van checkpoints kan volgen in een driedementionale ruimte. We hebben veel bijgeleerd over het trainen van agents en het ontwerpen van trainingsomgevingen samen met het ontwerpen van rewardfuncties. We zijn tevreden met het resultaat en zijn blij dit project af te ronden.

## Bronvermelding

- Simple Airplane Controller | Game Toolkits | Unity Asset Store. (2023, 11 mei). Unity Asset Store. https://assetstore.unity.com/packages/tools/game-toolkits/simple-airplane-controller-228575
- Midgard Skybox | 2D Sky | Unity Asset Store. (2024, 29 april). Unity Asset Store. https://assetstore.unity.com/packages/2d/textures-materials/sky/midgard-skybox-273733
- White City | 3D Urban | Unity Asset Store. (2018, 17 februari). Unity Asset Store. https://assetstore.unity.com/packages/3d/environments/urban/white-city-76766
- Free Checkpoint Sound Effect Download | SFX MP3 Library | Soundsnap. (z.d.). https://www.soundsnap.com/tags/checkpoint

### Project contributie

- Bers Goudantov (2ITAI)
- Vinnie Post (2ITAI)
- Lander Van der Stighelen (2ITAI)
