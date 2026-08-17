---
marp: true
theme: default
class: lead
paginate: true
header: 'System Architecture & Analysis'
footer: 'Marp Rendering Test Suite'
backgroundColor: '#f8f9fa'
size: 16:9
---

# 🚀 Advanced Embedded Systems
## Bridging Hardware and Software

**Rendering Test Suite**
*Checking directives, scoped styles, and layouts.*

---

# Core Concepts: Finite State Machines

Finite State Machines (FSMs) form the backbone of sequential digital logic design. Testing standard typography, bolding, and lists:

- **Moore Machines:** Output depends *only* on the current state.
- **Mealy Machines:** Output depends on current state **and** inputs.

> "A reliable state machine is the difference between a functional processor and an expensive space heater."

---

# Circuit Analysis: Mathematical Models

Testing inline and block LaTeX rendering. We often rely on Kirchhoff's Circuit Laws and Thevenin equivalents for nodal analysis.

**Kirchhoff's Voltage Law (KVL):**
The directed sum of the potential differences around any closed loop is zero. Testing inline math: $V_{total} = V_1 + V_2 + V_3$.

**Thevenin Equivalent Equations:**
Testing display math block:
$$V_{th} = V_{ab} = \lim_{R_L \to \infty} I_L R_L$$

$$I_{N} = \frac{V_{th}}{R_{th}}$$

---

<style scoped>
pre {
  background-color: #1e1e1e;
  color: #d4d4d4;
}
</style>

# Hardware Description: Code Blocks

Testing syntax highlighting and scoped CSS overrides for a dark-mode code block. Here is a basic VHDL entity:

```vhdl
library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

entity D_FlipFlop is
    Port ( clk : in STD_LOGIC;
           d   : in STD_LOGIC;
           q   : out STD_LOGIC);
end D_FlipFlop;