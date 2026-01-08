export default function PoolTabs({ activePool, setActivePool }) {
  const tabs = [
    { key: "technical", label: "Technical Pool" },
    { key: "review", label: "Review Pool" },
    { key: "hr", label: "HR Pool" },
    { key: "final", label: "Final Pool" },
    {key : "Sentoffer",label:"Sent Offer Pool"},
    {key : "Postoffer", label : "Post offer Pool"}
  ];

  return (
    <div className="flex border-b">
      {tabs.map((tab) => (
        <button
          key={tab.key}
          onClick={() => setActivePool(tab.key)}
          className={`px-6 py-3 font-medium transition
            ${
              activePool === tab.key
                ? "border-b-2 border-blue-600 text-blue-600"
                : "text-gray-500 hover:text-gray-700"
            }`}
        >
          {tab.label}
        </button>
      ))}
    </div>
  );
}
