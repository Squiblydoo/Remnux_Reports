# Malware Analysis Reports

A collection of static malware analysis reports produced on [REMnux](https://remnux.org/), an analyst-curated Linux distribution for reverse-engineering and malware analysis.

## Methodology

Reports are produced using an AI-assisted analysis workflow running on REMnux:

1. **Analyst direction** — the analyst submits a sample and drives the investigation
2. **AI agent execution** — [Claude Code](https://claude.ai/claude-code) selects tools, interprets output, and adapts the analysis strategy as findings emerge
3. **MCP server** — the [REMnux MCP server](https://github.com/REMnux/mcp-server) connects the AI agent to REMnux's 200+ analysis tools and encodes practitioner knowledge about which tools apply to which file types
4. **Isolated environment** — all execution and tool use occurs within REMnux; no samples touch the host

This workflow is described in detail by Lenny Zeltser in [*Using AI Agents to Analyze Malware on REMnux*](https://zeltser.com/ai-malware-analysis-remnux) (February 7, 2026).

## Scope and Limitations

- All analysis is **static** unless otherwise noted; behavioral conclusions are inferred from code, not live execution
- YARA family signature matches indicate *resemblance* to known families, not confirmed attribution
- C2 endpoints and infrastructure details were active at time of analysis and may have changed
- Reports reflect findings at the time of analysis and are not updated after publication

## Disclaimer

Samples and reports are shared for **defensive and educational purposes**. Do not execute samples outside a controlled analysis environment.
