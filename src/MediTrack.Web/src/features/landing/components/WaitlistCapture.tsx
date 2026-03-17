import { useState, type FormEvent } from "react";
import { Mail, CheckCircle } from "lucide-react";
import { clsxMerge } from "@/shared/utils/clsxMerge";

export function WaitlistCapture() {
  const [email, setEmail] = useState("");
  const [isSubmitted, setIsSubmitted] = useState(false);

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!email.trim()) return;
    // TODO: integrate with email collection backend
    setIsSubmitted(true);
  };

  return (
    <section className="border-y border-border bg-muted py-10 md:py-12">
      <div className="mx-auto max-w-xl px-4 text-center sm:px-6">
        {isSubmitted ? (
          <div className="flex flex-col items-center gap-2">
            <CheckCircle className="h-8 w-8 text-success-600" />
            <p className="text-base font-semibold text-foreground">
              You're on the list!
            </p>
            <p className="text-sm text-muted-foreground">
              We'll notify you when MediTrack is ready for early access.
            </p>
          </div>
        ) : (
          <>
            <h2 className="text-xl font-bold text-foreground sm:text-2xl">
              Not ready to dive in yet?
            </h2>
            <p className="mt-2 text-sm text-muted-foreground">
              Join the waitlist — we'll let you know when new features launch.
            </p>

            <form
              onSubmit={handleSubmit}
              className="mt-5 flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-center"
            >
              <div className="relative flex-1 sm:max-w-xs">
                <Mail className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground/70" />
                <input
                  type="email"
                  required
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                  placeholder="you@clinic.com"
                  autoComplete="email"
                  className={clsxMerge(
                    "h-10 w-full rounded-lg border border-border bg-card pl-9 pr-3",
                    "text-sm text-foreground placeholder:text-muted-foreground/70",
                    "focus:border-transparent focus:outline-none focus:ring-2 focus:ring-accent-500"
                  )}
                />
              </div>
              <button
                type="submit"
                className={clsxMerge(
                  "h-10 rounded-lg px-5",
                  "bg-accent-500 text-sm font-semibold text-white",
                  "transition-colors hover:bg-accent-600"
                )}
              >
                Join Waitlist
              </button>
            </form>

            <p className="mt-3 text-xs text-muted-foreground/70">
              No spam, ever. Unsubscribe anytime.
            </p>
          </>
        )}
      </div>
    </section>
  );
}
