import HeroPanel from "@/components/layout/HeroPanel";
import SignupForm from "@/components/auth/SignupForm";

const Signup = () => {
  return (
    <div className="min-h-screen bg-background flex">
      <aside className="hidden lg:block w-1/2 p-4 sticky top-0 h-screen">
        <HeroPanel />
      </aside>
      <div className="w-full lg:w-1/2 flex items-start lg:items-center justify-center pt-6 pb-10 lg:py-12 min-h-screen">
        <SignupForm />
      </div>
    </div>
  );
};

export default Signup;
