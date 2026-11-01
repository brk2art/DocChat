// The same shapes as DocChat.Core's AskResponse; ASP.NET Core sends them in camelCase.
export type SourceRef = { label: string; document: string; section: string; cited: boolean }

export type AskResponse = {
  answer: string
  sources: SourceRef[]
  inputTokens: number | null
  outputTokens: number | null
  durationMs: number
}

export async function ask(question: string): Promise<AskResponse> {
  const response = await fetch('/ask', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ question }),
  })

  if (!response.ok) {
    // The API answers 400 with { error: "..." } for an empty or too long question.
    const body = await response.json().catch(() => null)
    throw new Error(body?.error ?? `The server answered ${response.status}.`)
  }

  return response.json()
}