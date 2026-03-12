# Jennifer's Cannabis Empire — Game Design Spec
**Datum:** 2026-03-12
**Engine:** Unity 3D (URP — Universal Render Pipeline)
**Genre:** Single-Player Simulator
**Stil:** Low-Poly 3D, Third-Person

---

## Konzept

Ein Cannabis-Simulator in einer kleinen Low-Poly-Stadt. Der Spieler steuert Jennifer — eine wütende Unternehmerin die Cannabis anbaut, trocknet, verpackt und verkauft. Ähnlichkeiten zu Schedule I, mit einzigartigem Wut-Meter-System als Gameplay-Mechanik.

---

## Architektur & Projektstruktur

```
JenniferGame/
├── Assets/
│   ├── Characters/        # Jennifer + NPCs (Low-Poly Models)
│   ├── City/              # Gebäude, Straßen, Props
│   ├── Scripts/
│   │   ├── Core/          # GameManager, SaveSystem, TimeManager
│   │   ├── Jennifer/      # Movement, WutMeter, Animationen
│   │   ├── Systems/       # Anbau, Trocknen, Verpacken, Shop
│   │   ├── NPC/           # Kunden, Polizei, Mitarbeiter
│   │   └── UI/            # HUD, Menus, Dialoge
│   ├── Scenes/
│   │   ├── MainMenu
│   │   ├── City
│   │   └── Interior/      # Wohnung, Gewächshaus, Shop
│   └── ScriptableObjects/ # Items, Pflanzen, Rezepte
```

**Render Pipeline:** Unity URP (weiches Licht, Low-Poly-freundlich)
**Platform:** PC (Windows primär, Linux/Mac sekundär)
**Input System:** Unity Input System Package (neu, nicht legacy)
**Kamera:** Cinemachine Third-Person Camera (FreeLook)
**Scene-Übergänge:** Fade-to-Black via Trigger-Volumes beim Betreten von Türen (Voraussetzung für alle Interior-Szenen)
**Scene-Loading:** MVP: Single-Scene-Loading (einfacher zu debuggen); Post-MVP: Additive Scene Loading
**Save-System:** JSON-basiert, lokal gespeichert

**Win/Loss-Bedingung:**
- Endless Sandbox — kein definiertes Ende
- Loss: Nicht implementiert in MVP; Post-MVP optionaler Bankrott bei $0 + Schulden >$1.000
- Story-Ending: Post-MVP optionale Quest-Reihe

### Was wird gespeichert
- Geld, Inventar (Pflanzen, getrocknete Ware, Pakete inkl. Qualität)
- Pflanzenwachstum (Alter in Spieltagen, Bewässerungsstatus, letzter Bewässerungstag)
- Trocknungsfortschritt pro Item (Starttag, Qualitätsstufe)
- Wut-Level, Polizei-Aufmerksamkeit
- Aktuelle Spielzeit (Uhrzeit + Tag)
- Stufe (Progression), Schwarzmarkt-Freischaltflag
- Stammkunden-Kaufzähler (pro Kunde, 0–5)
- Mitarbeiter-Status (eingestellt/nicht eingestellt)
- Spieler-Position

---

## Zeit-System

| Parameter | Wert |
|-----------|------|
| 1 Spieltag | 20 Minuten Echtzeit |
| Tagesanfang | 06:00 Uhr |
| Nachtbeginn | 22:00 Uhr |
| Zeit im Interior | Läuft weiter (kein Pause) |

---

## Jennifer & Wut-Meter System

**Charakter:**
- Low-Poly 3D, Third-Person Kamera (leicht über der Schulter)
- Bewegung: WASD + Maus, Interaktion mit `E`
- Animationen: Laufen, Interagieren, Wütend-Ausbruch
- Design: Kurze Haare, verschränkte Arme in Idle, permanent genervter Gesichtsausdruck

**Wut-Meter:**

| Zustand | Wut-Level | Effekt |
|--------|-----------|--------|
| Entspannt | 0–30% | Normaler Betrieb |
| Gereizt | 31–60% | Dialoge werden aggressiver |
| Wütend | 61–85% | Einschüchtern möglich → +20% Preis-Bonus, aber 30% Chance Kunde flieht |
| Ausraster | 86–100% | 1 zufälliges Inventar-Item zerstört, Polizei-Aufmerksamkeit +15% |

**Wut steigt durch:**
- Polizei in Sichtweite: +5% pro 5 Sekunden
- Schwieriger Kunde (Haggling-Event): +10%
- Pflanze stirbt: +20%
- Fehlgeschlagene Aktion: +5%

**Wut sinkt durch:**
- Erfolgreicher Verkauf: -10%
- In Wohnung aufhalten: -2% pro 10 Sekunden (passiv)
- Geld einnehmen (Tagesziel erreicht): -15%

**Einschüchtern-Mechanik (Wütend-Zustand):**
- Taste `F` während Kundengespräch
- Dialogbox: Jennifer droht dem Kunden
- Ergebnis: 70% Erfolg (+20% Preis), 30% Kunde flieht + Wut +5%

**Feedback-Loop-Schutz:**
- Polizei-Aufmerksamkeit pausiert wenn Jennifer in der Wohnung ist
- Wut-Decke bei 100% — kein weiterer Anstieg, Ausraster-Event wird einmalig getriggert
- Nach Ausraster: Wut sinkt automatisch auf 70% (Ausraster-Cooldown), danach normaler Abbau
- 30 Sekunden Cooldown bevor nächster Ausraster möglich

---

## Kern-Gameplay Systeme

### 1. Anbau-System (Gewächshaus)

**Pflanzenstufen:**

| Stufe | Kosten | Wachstum | Ertrag | Qualität |
|-------|--------|----------|--------|----------|
| Billig | $50 | 3 Spieltage | 2 Einheiten | Niedrig |
| Normal | $150 | 5 Spieltage | 4 Einheiten | Mittel |
| Premium | $400 | 7 Spieltage | 6 Einheiten | Hoch |

**Bewässerung:**
- Einmal pro Spieltag gießen (zwischen 06:00–22:00)
- Nicht gegossen: Pflanze verliert 1 Wachstumstag
- 2 Tage nicht gegossen: Pflanze stirbt

### 2. Trocknen & Verpacken (Wohnung)

**Trocknung:**
- Dauer: 1 Spieltag pro Qualitätsstufe (Billig=1, Normal=2, Premium=3 Tage)
- Zu früh geerntet (Qualität sinkt um 1 Stufe)

**Qualitätsstufen:** Niedrig → Mittel → Hoch → Premium

**Qualitäts-Produktionspfad:**
- Billig-Pflanze → max. Niedrig
- Normal-Pflanze → max. Mittel
- Premium-Pflanze → max. Hoch nach Ernte
- Premium-Pflanze + perfekte Trocknung (volle Trocknungszeit, keine Unterbrechung) → Premium

**Verpackung:**

| Paketgröße | Benötigte Einheiten | Basispreis |
|-----------|---------------------|------------|
| Klein | 1 | $80–$120 |
| Mittel | 3 | $220–$350 |
| Groß | 6 | $400–$650 |

*Qualitätsmultiplikator: Niedrig ×0.7 / Mittel ×1.0 / Hoch ×1.3 / Premium ×1.6*

**Hinweis:** Premium-Große-Pakete (bis $1.040) überschreiten das Kundenbudget ($700) bewusst — Premium-Ware ist für den Schwarzmarkt (Stufe 3) gedacht. Reguläre Kunden kaufen max. Hoch-Qualität profitabel.
- Wenn Premium-Pakete vor Stufe 3 gepackt werden: sie lagern im Inventar und können nicht aufgelöst werden. UI zeigt einen Hinweis: "Schwarzmarkt-Kontakt nötig". Keine Fehlfunktion.

### 3. Shop-System

**Kunden-KI (State Machine):**
```
[Spawn an Shop-Tür] → [Warten (max. 60 Sek.)] → [Jennifer betritt Shop?]
       ↓ Nein nach 60s                                    ↓ Ja
  [Despawn, kommt morgen]                        [Kauf-Dialog öffnet]
                                                          ↓
                                           [Preis akzeptiert?] → [Kauf + Despawn]
                                                  ↓ Nein
                                           [Haggling-Event (1× pro Kunde)]
                                                  ↓ Nein
                                           [Kunde geht unverrichteter Dinge]
```

**Kunden-Schedule:**
- Tagsüber (08:00–20:00): 3–5 normale Kunden spawnen an der Shop-Tür
- Abends (20:00–22:00): 1–2 zahlungskräftigere Kunden
- Kunden haben Budgetlimit: Klein $150, Mittel $400, Groß $700
- Wenn Jennifer nicht im Shop: Kunden warten 60 Sekunden, dann Despawn

**Stammkunden:**
- Wird nach 5 erfolgreichen Käufen aktiviert
- Kommt täglich, kauft ohne zu verhandeln

**Shop-Interface:**
- Jennifer betritt den Shop → 2D UI-Overlay wird eingeblendet (Vogelperspektive auf die Theke)
- Drag & Drop Pakete aus Inventar auf Theke
- Preisslider pro Paket
- Kunde zeigt Reaktion (Emote über dem Kopf)

**Schwarzmarkt:** *(Stufe 3, nicht im MVP)*
- NPC-Kontakt in der Stadt
- Preise ×2, Polizei-Aufmerksamkeit +30% pro Deal

### 4. Polizei-System

**Aufmerksamkeits-Meter:**

| Level | Effekt |
|-------|--------|
| 0–40% | Keine Reaktion |
| 41–70% | Polizei patrouilliert öfter in der Nähe |
| 71–99% | Polizist spricht Jennifer an — Dialogwahl: zahlen ($500 Bestechung) oder fliehen *(Post-MVP)* |
| 100% | Razzia *(Post-MVP)* |

**MVP Polizei-Verhalten:** Meter sichtbar und funktional (steigt/fällt). Bei 100%: visuelles Warning-Flash + Wut +20%, Meter reset auf 50%. Bribe-Dialog und Razzia sind Post-MVP.

**Polizei-Patrouille:**
- 2 Polizisten laufen feste Route durch die Straße (Waypoint-System)
- Ab 41% Aufmerksamkeit: Route kommt näher am Shop vorbei

**Razzia:**
- 30% des Bargeldbestands als Strafe (Minimum $200)
- 50% des Inventars konfisziert
- Polizei-Aufmerksamkeit reset auf 20%
- 1 Spieltag Cooldown (Shop geschlossen)

**Polizei-Aufmerksamkeit sinkt durch:**
- Jeden Spieltag ohne Zwischenfall: -5%
- Bestechung ($500): -30%

### 5. Wirtschafts-Balance (Rechenbeispiel)

**Stufe 1 — Typischer Spieltag (1 Normal-Pflanze):**
- Normal-Pflanze: $150 Kosten, 5 Spieltage Wachstum, 2 Tage Trocknung = 7 Tage Zyklus
- Ertrag: 4 Einheiten → 1× Großes Paket ($220–$350, Mittel-Qualität ×1.0) + 1× Kleines Paket ($80–$120)
- Ø Einnahmen pro Zyklus: ~$400, Kosten: $150 → **Profit: ~$250 pro 7 Tagen**
- Stufe-2-Ziel ($5.000): ~20 Zyklen = 140 Spieltage (~47 Echtzeit-Stunden) — **zu langsam**

**Korrektur:** Stufe-2-Schwelle auf **$1.500** senken, Stufe-3 auf **$8.000**

| Stufe | Freischalt-Schwelle (korrigiert) | Ø Spieltage |
|-------|----------------------------------|-------------|
| 2 | $1.500 verdient gesamt | ~30 Tage |
| 3 | $8.000 verdient gesamt | ~80 Tage |

### 6. Progression

| Stufe | Freischaltbedingung | Neu verfügbar |
|-------|---------------------|---------------|
| 1 | Start | 1 Pflanze, kleiner Shop, 3 Stammkunden max |
| 2 | $1.500 verdient | 3 Pflanzen, 1 Mitarbeiter, größeres Gewächshaus |
| 3 | $8.000 verdient | Schwarzmarkt, Reputation-System, 6 Pflanzen |

**Mitarbeiter (Stufe 2):**
- Implementiert als Hintergrundprozess (kein physischer NPC) — bewässert alle Pflanzen einmal pro Spieltag automatisch
- Kosten: $200 pro Spieltag (wird automatisch vom Geld abgezogen)
- Kann über UI-Menü eingestellt/entlassen werden

**Reputation-System (Stufe 3):**
- Reputations-Punkte durch gute Deals, Stammkunden, pünktliche Lieferungen
- Hohe Reputation: bessere Kundenpreise, Schwarzmarkt-Kontakte
- Niedrige Reputation: weniger Kunden, misstrauische NPCs

---

## UI & Visuelles Design

**HUD:**
- Oben links: 💰 Geld, 📦 Paket-Anzahl
- Unten links: Wut-Meter (😊 → 😡) mit Farbverlauf grün→gelb→rot
- Unten rechts: Polizei-Aufmerksamkeit (👮)
- Oben rechts: Spielzeit-Uhr

**Low-Poly Stil (URP):**
- Farbpalette: Warme Grüntöne, Orange-Akzente, nächtlich-blaue Straßen
- Flat Lighting mit sanftem Rim-Light
- Keine normalen Schatten — Blob-Shadows unter Characters

**Stadt-Layout:**
```
[Polizei-Revier]      [Park]
       |                  |
[Straße ←————————————————→]
       |                  |
[Jennifers Shop]  [Schwarzmarkt-Kontakt]
       |
[Jennifers Wohnung]
       |
[Gewächshaus (hinten)]
```

**Menüs:**
- Hauptmenü: Dunkel, grüner Neon-Akzent, lo-fi Musik (lizenzfreie Assets von freesound.org)
- Inventar: Grid-System, 4×6 Slots
- Shop-Interface: 2D Overlay, Drag & Drop auf Theke

---

## MVP Scope

0. **Voraussetzung:** Fade-to-Black Szenenübergänge + Cinemachine Kamera (wird zuerst gebaut)
1. Hauptmenü, Pause-Menü, Quit-to-Desktop
2. Jennifer bewegt sich durch die Stadt (WASD + Cinemachine Third-Person)
3. Gewächshaus: 1 Billig-Pflanze anbauen, bewässern & ernten
4. Wohnung: Trocknen & Verpacken (Klein + Mittel)
5. Shop: Verkauf an 3 NPC-Kunden via UI-Overlay (Button-basiert, kein Drag & Drop im MVP)
6. Wut-Meter: UI + State Machine + Einschüchtern aktiv
7. Polizei: 2 Polizisten mit Waypoint-Patrol sichtbar in der Stadt → Wut steigt bei Sichtkontakt; Razzia-System ist Post-MVP
8. Geld-System + JSON-Speichern

**Audio MVP:** Lizenzfreie lo-fi Musik (freesound.org) im Hauptmenü + 3 SFX (Interaktion, Kauf, Wut-Ausraster)
