import HeroPanel from "@/components/layout/HeroPanel";
import LoginForm from "@/components/auth/LoginForm";

const Index = () => {
  return (
    <div className="min-h-screen bg-background flex">
      {/* Left hero - sticky */}
      <aside className="hidden lg:block w-1/2 p-4 sticky top-0 h-screen">
        <HeroPanel />
      </aside>

      {/* Right form */}
      <div className="w-full lg:w-1/2 flex items-start lg:items-center justify-center pt-6 pb-10 lg:py-12 min-h-screen">
        <LoginForm />
      </div>
    </div>
  );
};

export default Index;
