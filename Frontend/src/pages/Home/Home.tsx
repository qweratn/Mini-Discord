import { Button } from "@/components/ui/button";
import { Link } from "react-router";

export default function Home() {
  const info = [
    {
      img: "/users.svg",
      color: "bg-[#4742bf]",
      title: "Создавай группы",
      description: "Создавай группы и общайся с друзьями",
    },
    {
      img: "/chat.svg",
      color: "bg-[#316451]",
      title: "Общайся легко.",
      description: "Текстовые чаты в одном месте",
    },
    {
      img: "/shield.svg",
      color: "bg-[#5234b5]",
      title: "Будь в безопасности",
      description: "Защита данных и контроль доступа",
    },
    {
      img: "/lightning.svg",
      color: "bg-[#6277ef]",
      title: "Легко и быстро",
      description: "Максимальное удобство в использовании",
    },
  ];

  return (
    <main className="min-h-dvh overflow-x-hidden bg-[#0a0e20]">
      <div className="mx-auto flex min-h-dvh w-full max-w-6xl flex-col justify-center px-4 py-8 sm:px-6 sm:py-10 lg:px-8 lg:py-12">
        <section className="grid items-center gap-8 md:grid-cols-[minmax(0,0.85fr)_minmax(0,1.15fr)] lg:gap-12">
          <div className="flex flex-col items-start">
            <h1 className="text-4xl font-bold leading-[0.98] tracking-tight text-white sm:text-5xl lg:text-6xl">
              <span className="block">Общайся.</span>
              <span className="mt-1 block">Создавай.</span>
              <span className="mt-1 block text-[#6277ef]">Будь собой.</span>
            </h1>
            <p className="mt-5 max-w-md text-base leading-7 text-[#9d9faf] sm:text-lg">
              Mini Discord - твое пространство для общения, сообществ и идей.
            </p>
            <Link to="/sign-in" viewTransition className="w-full sm:w-auto">
              <Button
                variant="secondary"
                size="lg"
                className="mt-6 min-h-11 w-full px-8 sm:w-auto"
              >
                Войти
              </Button>
            </Link>
          </div>

          <div className="mx-auto w-full max-w-160">
            <img
              src="/hero.png"
              alt="Персонаж Mini Discord общается за ноутбуком"
              width={1536}
              height={1024}
              className="h-auto w-full object-contain"
            />
          </div>
        </section>

        <section
          aria-label="Возможности Mini Discord"
          className="mt-10 grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4"
        >
          {info.map((item) => (
            <div
              key={item.title}
              className="flex min-w-0 items-center gap-3 rounded-2xl border border-[#272737] bg-[#1e1e2f] p-3 sm:min-h-24 hover:scale-105 hover:shadow-md transition-transform duration-300"
            >
              <div
                className={
                  item.color +
                  " flex size-14 shrink-0 items-center justify-center rounded-full"
                }
              >
                <img
                  src={item.img}
                  alt=""
                  aria-hidden="true"
                  className="size-7 object-contain"
                />
              </div>
              <div className="min-w-0 flex-1">
                <h4 className="font-semibold leading-tight text-white">
                  {item.title}
                </h4>
                <p className="mt-1 wrap-break-word text-sm leading-5 text-[#9d9faf]">
                  {item.description}
                </p>
              </div>
            </div>
          ))}
        </section>
      </div>
    </main>
  );
}
