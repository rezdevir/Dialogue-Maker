# Dialogue Maker

## Version 0.1 (Multiple-Choice and Action Enabled and Style)

This app is designed to streamline collaboration between **writers** and **programmers**.  
Writers can easily:

- Create interactive, multiple-choice dialogues
- Switch between different branches
- Define specific in-game actions for each choice

For example, selecting an action in a dialogue can trigger events directly within the game.

---

## Screenshot

![bandicam 2025-09-01 23-11-42-148](https://github.com/user-attachments/assets/45ee2518-07de-4bed-97f5-d8d9fb7ce7cf)

![bandicam 2025-09-01 23-16-58-735](https://github.com/user-attachments/assets/6f0b5c76-960d-4e2f-a909-8ab243669672)

---

## Installation / Run

- The app executable can be found here:

bin/Debug/net8.0-windows/DialogMaker.exe

- All dialogues will be saved in:

bin/Debug/net8.0-windows/Dialogues

- Parameters must be saved in:

bin/Debug/net8.0-windows/parameters

---

## Parameters

### `speakers.txt`

Defines the list of available characters. Example:

Horse A

Horse B

Horse Black

### `emotions.txt`

Defines the emotions used in the game. Example:

nothing

happy

confuse

sad

---

## Notes

- **Actions** must be explained for the programmer in a **separate README file**.
- **Color** must be valid
