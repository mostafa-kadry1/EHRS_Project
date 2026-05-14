import { motion } from "framer-motion";
import { ShieldCheck, Activity, Heart } from "lucide-react";
import heroImage from "@/assets/medical-hero.jpg";
import { useLanguage } from "@/contexts/LanguageContext";

const HeroPanel = () => {
  const { t } = useLanguage();

  return (
    <div className="relative hidden lg:flex flex-col justify-between h-full overflow-hidden rounded-3xl">
      <img
        src={heroImage}
        alt="Medical background"
        className="absolute inset-0 w-full h-full object-cover"
      />
      <div className="absolute inset-0 bg-gradient-to-br from-[hsl(205,67%,22%,0.94)] via-[hsl(205,67%,28%,0.9)] to-[hsl(205,60%,18%,0.88)]" />

      <div className="relative z-10 flex flex-col justify-between h-full p-10">
        <div className="flex items-center gap-2.5">
          <div className="w-10 h-10 rounded-xl bg-card/15 backdrop-blur-sm flex items-center justify-center">
            <Activity size={22} className="text-card" />
          </div>
          <span className="font-display font-bold text-lg text-card">HealthCore EHRS</span>
        </div>

        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.7, delay: 0.2 }}
        >
          <h1 className="font-display text-4xl xl:text-5xl font-extrabold text-card leading-tight mb-4">
            {t.welcomeTo}<br />HealthCore
          </h1>
          <p className="text-card/70 text-lg max-w-sm leading-relaxed">
            {t.heroSubtitle}
          </p>
        </motion.div>

        <div className="flex flex-col gap-3">
          {[
            { icon: ShieldCheck, text: t.badge1 },
            { icon: Heart, text: t.badge2 },
            { icon: Activity, text: t.badge3 },
          ].map(({ icon: Icon, text }, i) => (
            <motion.div
              key={i}
              initial={{ opacity: 0, x: -20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ delay: 0.5 + i * 0.15 }}
              className="flex items-center gap-3"
            >
              <div className="w-8 h-8 rounded-lg bg-card/10 backdrop-blur-sm flex items-center justify-center">
                <Icon size={16} className="text-card/80" />
              </div>
              <span className="text-sm text-card/70 font-medium">{text}</span>
            </motion.div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default HeroPanel;
