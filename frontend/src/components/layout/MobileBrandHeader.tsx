import logo from "@/assets/healthcore-logo.png";
import LanguageToggle from "@/components/navigation/LanguageToggle";

const MobileBrandHeader = () => {
  return (
    <div className="lg:hidden flex items-center justify-between mb-5">
      <div className="flex items-center gap-3">
        <img
          src={logo}
          alt="HealthCore logo"
          width={44}
          height={44}
          loading="lazy"
          className="w-11 h-11 rounded-xl shadow-md object-contain"
        />
        <div className="flex flex-col leading-tight">
          <span className="font-display font-bold text-lg text-foreground">HealthCore EHRS</span>
        </div>
      </div>
      <LanguageToggle />
    </div>
  );
};

export default MobileBrandHeader;
