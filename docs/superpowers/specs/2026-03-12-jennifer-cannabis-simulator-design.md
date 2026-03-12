# Jennifer's Cannabis Empire — Game Design Spec
**Datum:** 2026-03-12
**Engine:** Unity 3D
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
│   │   ├── Core/          # GameManager, SaveSystem
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

**Save-System:** JSON-basiert, lokal gespeichert

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
| Wütend | 61–85% | Einschüchtern möglich → bessere Preise, aber Kunden fliehen |
| Ausraster | 86–100% | Ware zerstört, Polizei-Aufmerksamkeit steigt |

**Wut steigt durch:** Polizei in der Nähe, schwierige Kunden, fehlgeschlagene Aktionen
**Wut sinkt durch:** Erfolgreiche Verkäufe, Pause in der Wohnung, Geld einnehmen

---

## Kern-Gameplay Systeme

### 1. Anbau-System (Gewächshaus)
- Pflanzen kaufen → einpflanzen → täglich gießen → nach X Spieltagen ernten
- 3 Pflanzenstufen: Billig / Normal / Premium
- Zu wenig Wasser → Pflanze stirbt

### 2. Trocknen & Verpacken (Wohnung)
- Ernte → Trocknungsrack (Spielzeit-basiert)
- Getrocknet → Verpackungsstation → kleine / mittlere / große Pakete
- Qualität beeinflusst Verkaufspreis

### 3. Shop-System
- Kunden kommen zu festen Zeiten (Tag/Nacht-Zyklus)
- Preise selbst festlegen
- Stammkunden bei gutem Service
- Schwarzmarkt: höhere Preise, höheres Polizei-Risiko

### 4. Progression
- **Stufe 1:** 1 Pflanze, kleiner Shop
- **Stufe 2:** Gewächshaus upgraden, Mitarbeiter einstellen
- **Stufe 3:** Schwarzmarkt freischalten, Reputation-System aktiv

### 5. Polizei-System
- Aufmerksamkeits-Meter (steigt durch Ausraster, Schwarzmarkt)
- Bei 100%: Razzia → Verluste + Geldstrafe

---

## UI & Visuelles Design

**HUD:**
- Oben links: Geld, Paket-Anzahl
- Unten links: Wut-Meter (😊 → 😡)
- Unten rechts: Polizei-Aufmerksamkeit

**Low-Poly Stil:**
- Farbpalette: Warme Grüntöne, Orange-Akzente, nächtlich-blaue Straßen
- Weiches Licht, keine harten Schatten

**Stadt-Layout:**
```
[Polizei-Revier]  [Park]
      |               |
[Straße ←————————————→]
      |               |
[Jennifers Shop] [Schwarzmarkt-Kontakt]
      |
[Jennifers Wohnung + Gewächshaus]
```

**Menüs:**
- Hauptmenü: Dunkel, grüner Neon-Akzent, lo-fi Musik
- Inventar: Grid-System
- Shop-Interface: Drag & Drop Pakete auf Theke

---

## MVP Scope

1. Jennifer bewegt sich durch die Stadt (Third-Person)
2. Gewächshaus: 1 Pflanze anbauen & ernten
3. Wohnung: Trocknen & Verpacken
4. Shop: Verkauf an 3 NPC-Kunden
5. Wut-Meter funktioniert
6. Geld-System + einfaches Speichern
