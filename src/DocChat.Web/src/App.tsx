import { useState, type FormEvent } from 'react'
import { ask, type AskResponse } from './api'
import './App.css'

// One question and what came back for it.
type Turn = { id: number; question: string; response?: AskResponse; error?: string }

const examples = [
  'My parcel arrived broken. What should I do?',
  'Can I send a bottle of wine to my friend in Oslo?',
  'How long do I have to return a jacket I don\'t like?',
]

export default function App() {
  const [turns, setTurns] = useState<Turn[]>([])
  const [question, setQuestion] = useState('')
  const [busy, setBusy] = useState(false)

  async function submit(text: string) {
    const q = text.trim()
    if (!q || busy) return

    const id = Date.now()
    setTurns(t => [...t, { id, question: q }])
    setQuestion('')
    setBusy(true)
    try {
      const response = await ask(q)
      setTurns(t => t.map(x => (x.id === id ? { ...x, response } : x)))
    } catch (e) {
      const error = e instanceof Error ? e.message : 'Something went wrong.'
      setTurns(t => t.map(x => (x.id === id ? { ...x, error } : x)))
    } finally {
      setBusy(false)
    }
  }

  function onSubmit(e: FormEvent) {
    e.preventDefault() // a form would otherwise reload the page
    void submit(question)
  }

  return (
    <div className="page">
      <header>
        <h1>Brightway Parcels help</h1>
        <p>Answers come only from our policy documents, with their sources. Brightway Parcels is a fictional company.</p>
      </header>

      <main>
        {turns.length === 0 && (
          <div className="examples">
            <p>Try one of these:</p>
            {examples.map(x => (
              <button key={x} type="button" onClick={() => void submit(x)}>{x}</button>
            ))}
          </div>
        )}
        {turns.map(t => <TurnView key={t.id} turn={t} />)}
      </main>

      <form onSubmit={onSubmit}>
        <input
          value={question}
          onChange={e => setQuestion(e.target.value)}
          placeholder="Ask about delivery, returns, damage claims…"
          maxLength={500}
          disabled={busy}
        />
        <button type="submit" disabled={busy || !question.trim()}>{busy ? 'Thinking…' : 'Ask'}</button>
      </form>
    </div>
  )
}

function TurnView({ turn }: { turn: Turn }) {
  // Only the sources the answer really used: the API marks them, checked in code.
  const cited = turn.response?.sources.filter(s => s.cited) ?? []

  return (
    <section className="turn">
      <p className="question">{turn.question}</p>

      {!turn.response && !turn.error && <p className="pending">Searching the policies…</p>}
      {turn.error && <p className="error">{turn.error}</p>}

      {turn.response && (
        <>
          <p className="answer">{withCitations(turn.response.answer)}</p>
          {cited.length > 0 && (
            <ul className="sources">
              {cited.map(s => (
                <li key={s.label}>
                  <span className="label">{s.label}</span> {s.document.replace('.md', '')} › {s.section}
                </li>
              ))}
            </ul>
          )}
          <p className="meta">
            {(turn.response.durationMs / 1000).toFixed(1)} s · {turn.response.inputTokens ?? '?'} in, {turn.response.outputTokens ?? '?'} out tokens
          </p>
        </>
      )}
    </section>
  )
}

// "…within 14 days [S1]." → the [S1] becomes a small badge that matches the source list.
function withCitations(text: string) {
  return text.split(/(\[S\d+\])/g).map((part, i) =>
    /^\[S\d+\]$/.test(part) ? <sup key={i} className="cite">{part.slice(1, -1)}</sup> : part,
  )
}